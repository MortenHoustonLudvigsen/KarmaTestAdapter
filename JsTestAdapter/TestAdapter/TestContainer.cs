using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using JsTestAdapter.TestServerClient;
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

namespace JsTestAdapter.TestAdapter
{
    public abstract class TestContainer : TestContainerBase
    {
        public TestContainer(TestContainerList containers, IVsProject project, string source)
            : base(containers.Discoverer, source)
        {
            Project = project;
            ProjectDirectory = project.GetProjectDir();
            BaseDirectory = Discoverer.BaseDirectory;
            Name = string.Join("/", new[] {
                Project.GetProjectName(),
                Path.GetDirectoryName(GetRelativePath(Source)).Replace('\\', '/')
            }.Where(s => !string.IsNullOrWhiteSpace(s)));
            Logger = new TestLogger(Discoverer.Logger, Name);
            ServerLogger = CreateServerLogger();
            Logger.Debug("Creating TestContainer for {0}", GetRelativePath(Source));
            Containers = containers;
            SourceSettings = Discoverer.TestSettings.AddSource(Name, Source);
            try
            {
                Init();
            }
            catch (Exception ex)
            {
                Validate(false, "Error: " + ex.Message);
                Logger.Error(ex, "Could not load tests");
            }
            FileWatchers = GetFileWatchers().Where(f => f != null).ToList();
            if (IsValid)
            {
                SourceSettingsPersister.Save(Discoverer.TestAdapterInfo.SettingsFileDirectory, SourceSettings);
                StartTestServer();
            }
            RefreshContainer("TestContainer created");
            if (!IsValid)
            {
                Logger.Warn(InvalidReason);
            }
            else
            {
                Logger.Info("TestContainer created");
            }
        }

        protected abstract void Init();
        protected abstract TestServerLogger CreateServerLogger();
        protected abstract JsTestAdapter.TestServerClient.TestServer CreateTestServer();

        public IVsProject Project { get; private set; }
        public string ProjectDirectory { get; private set; }
        public string Name { get; private set; }
        public TestContainerList Containers { get; private set; }
        public ITestLogger Logger { get; private set; }
        public TestServerLogger ServerLogger { get; private set; }
        public JsTestAdapter.TestServerClient.TestServer TestServer { get; private set; }
        public EventCommand EventCommand { get; private set; }
        public TestSourceSettings SourceSettings { get; private set; }
        public Task EventsTask { get; private set; }
        public int Port { get; private set; }
        public string BaseDirectory { get; private set; }
        public IEnumerable<FileWatcher> FileWatchers { get; private set; }
        public IEnumerable<Guid> Tests { get; private set; }

        private readonly Validator _validator = new Validator();
        public bool IsValid { get { return _validator.IsValid; } }
        public string InvalidReason { get { return _validator.InvalidReason; } }

        public void Validate(bool isValid, string reasonFmt, params object[] args)
        {
            _validator.Validate(isValid, reasonFmt, args);
        }

        public void Validate(bool isValid, string reason = null)
        {
            _validator.Validate(isValid, reason);
        }

        public virtual int Priority { get { return Discoverer.TestAdapterInfo.GetContainerPriority(this.Source); } }

        public virtual bool HasFile(string file)
        {
            return PathUtils.PathsEqual(file, Source);
        }

        public virtual bool IsDuplicate(TestContainer other)
        {
            if (other != this && this.HasFile(other.Source))
            {
                return this.Priority > other.Priority;
            }
            return false;
        }

        public virtual IEnumerable<string> GetFilesToWatch()
        {
            yield return Source;
        }

        private IEnumerable<FileWatcher> GetFileWatchers()
        {
            return GetFilesToWatch().Select(f => CreateFileWatcher(f)).ToList();
        }

        private FileWatcher CreateFileWatcher(string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                return CreateFileWatcher(Path.GetDirectoryName(file), Path.GetFileName(file), false);
            }
            return null;
        }

        private FileWatcher CreateFileWatcher(string directory, string filter, bool includeSubdirectories)
        {
            if (Directory.Exists(directory))
            {
                var watcher = new FileWatcher(directory, filter, includeSubdirectories);
                watcher.Changed += FileWatcherChanged;
                Logger.Debug(@"Watching {0}", GetRelativePath(watcher.Watching));
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
                    Containers.CreateContainer(Discoverer.CreateTestContainerSource(Project, Source));
                    break;
                case FileChangedReason.Removed:
                    Containers.Remove(this);
                    break;
            }
        }

        private void StartTestServer()
        {
            try
            {
                TestServer = CreateTestServer();
                TestServer.OutputReceived += line => ServerLogger.Log(line);
                TestServer.ErrorReceived += line => ServerLogger.Error(line);
                TestServer.Started += port => OnServerStarted(port);
                TestServer.Stopped += (exitCode, ex) => OnServerStopped(exitCode, ex);
                TestServer.StartServer(); // No timeout
            }
            catch (Exception ex)
            {
                Validate(false, "Could not start {0}: {1}", Discoverer.TestAdapterInfo.Name, ex.Message);
                Logger.Error(ex, "Could not start {0}", Discoverer.TestAdapterInfo.Name);
            }
        }

        private void StopTestServer(string reason, bool warn)
        {
            if (TestServer != null)
            {
                Validate(false, "Stopping {0}: {1}", Discoverer.TestAdapterInfo.Name, reason);
                TestServer.Kill(reason, warn);
            }
        }

        private void OnServerStarted(int port)
        {
            Logger.Debug("{0} started using port {1}", Discoverer.TestAdapterInfo.Name, port);
            TestServer.Attempts = 0;
            Port = port;
            SourceSettings.Port = port;
            EventCommand = new EventCommand(port);
            EventsTask = EventCommand.Run(OnServerEvent);
            RefreshContainer(string.Format("{0} started for {1}", Discoverer.TestAdapterInfo.Name, Name));
        }

        private void OnServerStopped(int? exitCode, Exception ex)
        {
            if (ex != null)
            {
                Logger.Error(ex, "{0} stopped", Discoverer.TestAdapterInfo.Name);
            }
            else if (IsValid)
            {
                Logger.Warn("{0} stopped - exit code: {1}", Discoverer.TestAdapterInfo.Name, exitCode);
                if (TestServer.Attempts < 3)
                {
                    Logger.Warn("Restarting {0}", Discoverer.TestAdapterInfo.Name);
                    Task.Delay(250).ContinueWith(t => StartTestServer());
                }
                else
                {
                    Validate(false, "Could not start {0} after {1} attempts", Discoverer.TestAdapterInfo.Name, TestServer.Attempts);
                    Logger.Error(InvalidReason);
                    RefreshContainer("");
                }
            }
            else
            {
                Logger.Debug("{0} stopped - exit code: {1}", Discoverer.TestAdapterInfo.Name, exitCode);
            }
        }

        private bool _refreshing = false;
        private void RefreshContainer(string reason)
        {
            _refreshing = true;
            TimeStamp = DateTime.Now;
            if (IsValid)
            {
                SourceSettingsPersister.Save(Discoverer.TestAdapterInfo.SettingsFileDirectory, SourceSettings);
            }
            Discoverer.RefreshTestContainers(reason);
        }

        private void OnServerEvent(ServerEvent evt)
        {
            switch (evt.Event)
            {
                case "Test run start":
                    RefreshContainer(string.Format("Test run started for {0}", Name));
                    break;
                case "Test run requested":
                    if (IsValid && _refreshing)
                    {
                        _refreshing = false;
                        Tests = evt.Tests.ToList();
                        Discoverer.RunTests();
                    }
                    break;
            }
        }

        public string GetRelativePath(string path)
        {
            return PathUtils.GetRelativePath(Project.GetProjectDir(), path).Replace('\\', '/');
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
                Logger.Debug("Disposing of TestContainer");
                StopTestServer("Disposing", false);

                if (FileWatchers != null)
                {
                    foreach (var watcher in FileWatchers)
                    {
                        Logger.Debug(@"Stop watching {0}", GetRelativePath(watcher.Watching));
                        watcher.Dispose();
                    }
                    FileWatchers = null;
                }

                SourceSettingsPersister.DeleteSettingsFile(Discoverer.TestAdapterInfo.SettingsFileDirectory, SourceSettings);
                Containers.Remove(this);
            }

            Validate(false, "Disposing");
        }

        #endregion
    }
}