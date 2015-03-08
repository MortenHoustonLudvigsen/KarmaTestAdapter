using KarmaTestAdapter.Karma;
using KarmaTestAdapter.Logging;
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
        public KarmaTestContainer(KarmaTestContainerList containers, string source)
            : base(containers.Discoverer, source)
        {
            Logger = new KarmaLogger(Discoverer.Logger, "Container");
            KarmaLogger = new KarmaLogger(Logger, "Karma");
            Logger.Info("Creating KarmaTestContainer for {0}", PathUtils.GetRelativePath(BaseDirectory, Source));
            Containers = containers;
            KarmaSourceSettings = Discoverer.TestSettings.AddSource(Source);
            try
            {
                Settings = new KarmaSettings(Source, Logger);
                SetIsValid(Settings.AreValid, "Settings are not valid");
            }
            catch (Exception ex)
            {
                SetIsValid(false, "Error: " + ex.Message);
                Logger.Error(ex, string.Format("Could not load tests from {0}", PathUtils.GetRelativePath(BaseDirectory, source)));
            }
            FileWatchers = GetFileWatchers().ToList();
            if (IsValid)
            {
                KarmaSourceSettings = Discoverer.TestSettings.AddSource(Source);
                KarmaSourceSettings.BaseDirectory = BaseDirectory;
                StartKarmaServer();
            }
            Logger.Info("{0} KarmaTestContainer created for {1}", IsValid ? "Valid" : "Invalid", PathUtils.GetRelativePath(BaseDirectory, Source));
        }

        public KarmaTestContainerList Containers { get; private set; }
        public IKarmaLogger Logger { get; private set; }
        public IKarmaLogger KarmaLogger { get; private set; }
        public KarmaSettings Settings { get; private set; }
        public KarmaServer KarmaServer { get; private set; }
        public KarmaEventCommand KarmaEventCommand { get; private set; }
        public KarmaSourceSettings KarmaSourceSettings { get; private set; }
        public Task KarmaEvents { get; private set; }
        public int Port { get; private set; }
        public string BaseDirectory { get { return Discoverer.BaseDirectory; } }
        public IEnumerable<KarmaFileWatcher> FileWatchers { get; private set; }

        public bool IsValid { get; private set; }
        public string InvalidReason { get; private set; }
        private void SetIsValid(bool isValid, string reason)
        {
            IsValid = isValid;
            if (IsValid)
            {
                InvalidReason = "";
            }
            else if (string.IsNullOrWhiteSpace(InvalidReason))
            {
                InvalidReason = reason;
            }
        }

        private IEnumerable<KarmaFileWatcher> GetFileWatchers()
        {
            if (IsValid)
            {
                yield return CreateFileWatcher(Settings.SettingsFile);
                yield return CreateFileWatcher(Settings.KarmaConfigFile);
            }
            else
            {
                yield return CreateFileWatcher(Source);
            }
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
            var watcher = new KarmaFileWatcher(directory, filter, includeSubdirectories);
            watcher.Changed += FileWatcherChanged;
            Logger.Info(@"Watching {0}", PathUtils.GetRelativePath(BaseDirectory, watcher.Watching));
            return watcher;
        }

        private void FileWatcherChanged(object sender, FileChangedEventArgs e)
        {
            Logger.Debug("File {0}: {1}", e.ChangedReason, PathUtils.GetRelativePath(BaseDirectory, e.File));
            switch (e.ChangedReason)
            {
                case FileChangedReason.Added:
                    if (PathUtils.PathsEqual(e.File, Settings.SettingsFile))
                    {
                        Containers.CreateContainer(e.File);
                    }
                    else
                    {
                        Containers.CreateContainer(Source);
                    }
                    break;
                case FileChangedReason.Changed:
                case FileChangedReason.Saved:
                    Containers.CreateContainer(Source);
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
                KarmaServer.OutputReceived += line => KarmaLogger.Info(line);
                KarmaServer.ErrorReceived += line => KarmaLogger.Error(line);
                KarmaServer.Started += port => OnServerStarted(port);
                KarmaServer.Stopped += (exitCode, ex) => OnServerStopped(exitCode, ex);
                KarmaServer.StartServer(10000); // Timeout after 10 seconds
            }
            catch (Exception ex)
            {
                SetIsValid(false, "Could not start karma: " + ex.Message);
                Logger.Error(ex, "Could not start karma");
            }
        }

        private void StopKarmaServer(string reason)
        {
            SetIsValid(false, "Stopping karma: " + reason);
            KarmaServer.Kill(reason);
        }

        private void OnServerStarted(int port)
        {
            Logger.Debug("Karma started using port {0}", port);
            Port = port;
            KarmaSourceSettings.Port = port;
            KarmaEventCommand = new KarmaEventCommand(port);
            KarmaEventCommand.Connected += () => Logger.Info("Listening to karma events");
            KarmaEventCommand.Disconnected += () => Logger.Info("Stopped listening to karma events");
            KarmaEvents = KarmaEventCommand.Run(OnKarmaEvent);
            RefreshContainer("Karma started");
        }

        private void OnServerStopped(int? exitCode, Exception ex)
        {
            SetIsValid(false, "Karma stopped");
            if (ex != null)
            {
                Logger.Error(ex, "Karma stopped");
            }
            else
            {
                Logger.Debug("Karma stopped - exit code: {0}", exitCode);
            }
            RefreshContainer("Karma stopped");
        }

        private bool _refreshing = false;
        private void RefreshContainer(string reason)
        {
            _refreshing = true;
            TimeStamp = DateTime.Now;
            Discoverer.RefreshTestContainers(reason);
        }

        private void OnKarmaEvent(KarmaEvent evt)
        {
            Logger.Debug("Event: {0}", evt);
            switch (evt.Event)
            {
                case "Karma run start":
                    RefreshContainer("Karma run started");
                    break;
                case "Karma run requested":
                    if (IsValid && _refreshing)
                    {
                        _refreshing = false;
                        Discoverer.RequestFactory.ExecuteTestsAsync(evt.Tests, null);
                    }
                    break;
            }
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
                Logger.Debug("Disposing of KarmaTestContainer for {0}", Source);
                StopKarmaServer("Disposing");
                
                if (FileWatchers != null)
                {
                    foreach (var watcher in FileWatchers)
                    {
                        Logger.Debug(@"Stop watching {0}", PathUtils.GetRelativePath(BaseDirectory, watcher.Watching, true));
                        watcher.Dispose();
                    }
                    FileWatchers = null;
                }

                Containers.Remove(this);
            }

            SetIsValid(false, "Disposing");
        }

        #endregion
    }
}