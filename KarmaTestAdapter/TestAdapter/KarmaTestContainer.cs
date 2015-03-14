using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Karma;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestContainer : KarmaTestContainerBase<KarmaTestContainerDiscoverer>
    {
        public KarmaTestContainer(KarmaTestContainerList containers, IVsProject project, string source)
            : base(containers.Discoverer, source)
        {
            Project = project;
            ProjectDirectory = project.GetProjectDir();
            BaseDirectory = Discoverer.BaseDirectory;
            Name = string.Join("/", new[] {
                Project.GetProjectName(),
                Path.GetDirectoryName(GetRelativePath(Source)).Replace('\\', '/')
            }.Where(s => !string.IsNullOrWhiteSpace(s)));
            Logger = new KarmaLogger(Discoverer.Logger, Name);
            KarmaLogger = new KarmaServerLogger(Logger);
            Logger.Info("Creating KarmaTestContainer for {0}", GetRelativePath(Source));
            Containers = containers;
            KarmaSourceSettings = Discoverer.TestSettings.AddSource(Name, Source);
            try
            {
                Settings = new KarmaSettings(Source, f => File.Exists(f), BaseDirectory, Logger);
                _validator.Validate(Settings.AreValid, Settings.InvalidReason);
                if (Settings.AreValid)
                {
                    if (Settings.HasSettingsFile)
                    {
                        _validator.Validate(Project.HasFile(Settings.SettingsFile), "File {1} is not included in project {0}", Project.GetProjectName(), GetRelativePath(Settings.SettingsFile));
                        _validator.Validate(Discoverer.ServiceProvider.HasFile(Settings.KarmaConfigFile), "File {0} is not included in solution", GetRelativePath(Settings.KarmaConfigFile));
                    }
                    else
                    {
                        _validator.Validate(Project.HasFile(Settings.KarmaConfigFile), "File {1} is not included in project {0}", Project.GetProjectName(), GetRelativePath(Settings.KarmaConfigFile));
                    }
                }

                if (Settings.Disabled)
                {
                    _validator.Validate(false, string.Format("Karma is disabled in {0}", GetRelativePath(Settings.SettingsFile)));
                }
            }
            catch (Exception ex)
            {
                _validator.Validate(false, "Error: " + ex.Message);
                Logger.Error(ex, "Could not load tests");
            }
            FileWatchers = GetFileWatchers().Where(f => f != null).ToList();
            if (IsValid)
            {
                KarmaSourceSettings.Save();
                StartKarmaServer();
            }
            RefreshContainer("KarmaTestContainer created");
            if (!IsValid)
            {
                Logger.Warn(InvalidReason);
            }
            else
            {
                Logger.Info("KarmaTestContainer created");
            }
        }

        public IVsProject Project { get; private set; }
        public string ProjectDirectory { get; private set; }
        public string Name { get; private set; }
        public KarmaTestContainerList Containers { get; private set; }
        public IKarmaLogger Logger { get; private set; }
        public KarmaServerLogger KarmaLogger { get; private set; }
        public KarmaSettings Settings { get; private set; }
        public KarmaServer KarmaServer { get; private set; }
        public KarmaEventCommand KarmaEventCommand { get; private set; }
        public KarmaSourceSettings KarmaSourceSettings { get; private set; }
        public Task KarmaEvents { get; private set; }
        public int Port { get; private set; }
        public string BaseDirectory { get; private set; }
        public IEnumerable<KarmaFileWatcher> FileWatchers { get; private set; }
        public IEnumerable<Guid> Tests { get; private set; }

        private readonly Validator _validator = new Validator();
        public bool IsValid { get { return _validator.IsValid; } }
        public string InvalidReason { get { return _validator.InvalidReason; } }

        public bool HasFile(string file)
        {
            return PathUtils.PathsEqual(file, Settings.SettingsFile) || PathUtils.PathsEqual(file, Settings.KarmaConfigFile);
        }

        private IEnumerable<KarmaFileWatcher> GetFileWatchers()
        {
            yield return CreateFileWatcher(Settings.SettingsFile);
            yield return CreateFileWatcher(Settings.KarmaConfigFile);
        }

        private KarmaFileWatcher CreateFileWatcher(string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                return CreateFileWatcher(Path.GetDirectoryName(file), Path.GetFileName(file), false);
            }
            return null;
        }

        private KarmaFileWatcher CreateFileWatcher(string directory, string filter, bool includeSubdirectories)
        {
            if (Directory.Exists(directory))
            {
                var watcher = new KarmaFileWatcher(directory, filter, includeSubdirectories);
                watcher.Changed += FileWatcherChanged;
                Logger.Info(@"Watching {0}", GetRelativePath(watcher.Watching));
                return watcher;
            }
            return null;
        }

        private void FileWatcherChanged(object sender, FileChangedEventArgs e)
        {
            Logger.Debug("File {0}: {1}", e.ChangedReason, GetRelativePath(e.File));
            switch (e.ChangedReason)
            {
                case FileChangedReason.Changed:
                    Containers.CreateContainer(new KarmaTestContainerSourceInfo(Project, Source));
                    break;
                case FileChangedReason.Removed:
                    Containers.Remove(this);
                    break;
            }
        }

        private void StartKarmaServer()
        {
            try
            {
                KarmaServer = new KarmaServer(Settings, Logger);
                KarmaServer.OutputReceived += line => KarmaLogger.Log(line);
                KarmaServer.ErrorReceived += line => KarmaLogger.Error(line);
                KarmaServer.Started += port => OnServerStarted(port);
                KarmaServer.Stopped += (exitCode, ex) => OnServerStopped(exitCode, ex);
                KarmaServer.StartServer(); // No timeout
            }
            catch (Exception ex)
            {
                _validator.Validate(false, "Could not start karma: {0}", ex.Message);
                Logger.Error(ex, "Could not start karma");
            }
        }

        private void StopKarmaServer(string reason, bool warn)
        {
            if (KarmaServer != null)
            {
                _validator.Validate(false, "Stopping karma: {0}", reason);
                KarmaServer.Kill(reason, warn);
            }
        }

        private void OnServerStarted(int port)
        {
            Logger.Debug("Karma started using port {0}", port);
            Port = port;
            KarmaSourceSettings.Port = port;
            KarmaEventCommand = new KarmaEventCommand(port);
            KarmaEvents = KarmaEventCommand.Run(OnKarmaEvent);
            RefreshContainer(string.Format("Karma started for {0}", Name));
        }

        private void OnServerStopped(int? exitCode, Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex, "Karma stopped");
            }
            else if (IsValid)
            {
                Logger.Warn("Karma stopped - exit code: {0}", exitCode);
                Logger.Warn("Restarting karma");
                Task.Delay(250).ContinueWith(t => StartKarmaServer());
            }
            else
            {
                Logger.Debug("Karma stopped - exit code: {0}", exitCode);
            }
        }

        private bool _refreshing = false;
        private void RefreshContainer(string reason)
        {
            _refreshing = true;
            TimeStamp = DateTime.Now;
            if (IsValid)
            {
                KarmaSourceSettings.Save();
            }
            Discoverer.RefreshTestContainers(reason);
        }

        private void OnKarmaEvent(KarmaEvent evt)
        {
            switch (evt.Event)
            {
                case "Karma run start":
                    RefreshContainer(string.Format("Karma run started for {0}", Name));
                    break;
                case "Karma run requested":
                    if (IsValid && _refreshing)
                    {
                        _refreshing = false;
                        Tests = evt.Tests.ToList();
                        Discoverer.RunTests();
                    }
                    break;
            }
        }

        private string GetRelativePath(string path)
        {
            return PathUtils.GetRelativePath(Project.GetProjectDir(), path);
        }

        public override string ToString()
        {
            var str = IsValid ? "Valid" : "Invalid";
            if (IsValid)
            {
                if (Port > 0)
                {
                    str += string.Format(" (connected to {0})", Port);
                }
                else
                {
                    str += " (not connected)";
                }
            }
            else
            {
                str += " (" + InvalidReason + ")";
            }
            str += ": " + Source;
            return str;
        }

        #region IDisposable

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                Logger.Debug("Disposing of KarmaTestContainer");
                StopKarmaServer("Disposing", false);

                if (FileWatchers != null)
                {
                    foreach (var watcher in FileWatchers)
                    {
                        Logger.Debug(@"Stop watching {0}", GetRelativePath(watcher.Watching));
                        watcher.Dispose();
                    }
                    FileWatchers = null;
                }

                KarmaSourceSettings.DeleteSettingsFile();
                Containers.Remove(this);
            }

            _validator.Validate(false, "Disposing");
        }

        #endregion
    }
}