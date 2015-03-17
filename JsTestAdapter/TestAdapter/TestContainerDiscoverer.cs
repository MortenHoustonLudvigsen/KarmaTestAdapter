using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TPL = System.Threading.Tasks;

namespace JsTestAdapter.TestAdapter
{
    public abstract class TestContainerDiscoverer : ITestContainerDiscoverer, IDisposable
    {
        protected TestContainerDiscoverer(
            IServiceProvider serviceProvider,
            ITestsService testsService,
            TestSettingsProvider testSettingsProvider,
            ITestLogger logger
            )
        {
            Logger = logger;
            try
            {
                TestSettingsProvider = testSettingsProvider;
                TestAdapterInfo = CreateTestAdapterInfo();

                Containers = new TestContainerList(this);

                ServiceProvider = serviceProvider;
                _testsService = testsService;

                SolutionListener = new SolutionListener(serviceProvider);
                SolutionListener.SolutionLoaded += OnSolutionLoaded;
                SolutionListener.SolutionUnloaded += OnSolutionUnloaded;
                SolutionListener.ProjectChanged += OnSolutionProjectChanged;
                SolutionListener.StartListening();

                ProjectListener = new ProjectListener(serviceProvider, Logger);
                ProjectListener.FileAdded += (source, e) => OnProjectFileAdded(e.Project, e.File);
                ProjectListener.FileRemoved += (source, e) => OnProjectFileRemoved(e.Project, e.File);
                ProjectListener.FileRenamed += (source, e) => OnProjectFileRenamed(e.Project, e.OldFile, e.NewFile);
                ProjectListener.StartListening();

                Logger.Info("TestContainerDiscoverer created");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
        }

        public abstract TestAdapterInfo CreateTestAdapterInfo();
        public abstract TestContainer CreateTestContainer(TestContainerSource source);
        public abstract TestContainerSource CreateTestContainerSource(IVsProject project, string source);

        private bool _initialContainerSearch = true;
        private ITestsService _testsService;

        public Uri ExecutorUri { get { return TestAdapterInfo.ExecutorUri; } }
        public TestAdapterInfo TestAdapterInfo { get; private set; }
        public TestContainerList Containers { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public ISolutionListener SolutionListener { get; private set; }
        public IProjectListener ProjectListener { get; private set; }
        public ITestLogger Logger { get; set; }
        public TestSettingsProvider TestSettingsProvider { get; private set; }
        public TestSettings TestSettings { get { return TestSettingsProvider.Settings; } }
        public string BaseDirectory { get { return ServiceProvider.GetSolutionDirectory(); } }

        private void RunTestsInternal()
        {
            if (_testsService != null)
            {
                _testsService.RunTestsAsync(Containers.Where(c => c.IsValid && c.Tests != null).SelectMany(c => c.Tests));
            }
        }

        private bool _shouldRun = false;
        private object _runLock = new object();
        public void RunTests()
        {
            lock (_runLock)
            {
                _shouldRun = true;
            }
            TPL.Task.Delay(500).ContinueWith(t =>
            {
                lock (_runLock)
                {
                    if (_shouldRun)
                    {
                        _shouldRun = false;
                        RunTestsInternal();
                    }
                }
            });
        }

        public IEnumerable<ITestContainer> TestContainers
        {
            get
            {
                if (_initialContainerSearch)
                {
                    Containers.Clear();
                    Containers.CreateContainers(FindSources());
                    _initialContainerSearch = false;
                }
                return Containers.Where(c => c.IsValid && c.Port > 0);
            }
        }

        public event EventHandler TestContainersUpdated;
        private void OnTestContainersChanged()
        {
            if (TestContainersUpdated != null && !_initialContainerSearch)
            {
                TestContainersUpdated(this, EventArgs.Empty);
            }
        }

        private void OnSolutionLoaded(object sender, EventArgs e)
        {
            Logger.Info("Solution loaded");
            _initialContainerSearch = true;
            Containers.Clear();
        }

        private void OnSolutionUnloaded(object sender, EventArgs e)
        {
            Logger.Info("Solution unloaded");
            _initialContainerSearch = true;
            Containers.Clear();
        }

        private void OnSolutionProjectChanged(object sender, SolutionListenerEventArgs e)
        {
            if (e != null)
            {
                switch (e.ChangedReason)
                {
                    case SolutionChangedReason.Load:
                        Containers.CreateContainers(FindSources(e.Project));
                        RefreshTestContainers(string.Format("{0} loaded", e.Project.GetProjectName()));
                        break;
                    case SolutionChangedReason.Unload:
                        Containers.Remove(e.Project);
                        RefreshTestContainers(string.Format("{0} unloaded", e.Project.GetProjectName()));
                        break;
                    case SolutionChangedReason.Open:
                        Containers.CreateContainers(FindSources(e.Project));
                        RefreshTestContainers(string.Format("{0} opened", e.Project.GetProjectName()));
                        break;
                    case SolutionChangedReason.Close:
                        Containers.Remove(e.Project);
                        RefreshTestContainers(string.Format("{0} closed", e.Project.GetProjectName()));
                        break;
                }
            }
        }

        private void OnProjectFileAdded(IVsProject project, string file)
        {
            Logger.Debug("Project file added: {0}", file);
            if (TestAdapterInfo.IsTestContainer(file) || Containers.Any(c => c.HasFile(file)))
            {
                Containers.CreateContainer(CreateTestContainerSource(project, file));
            }
        }

        private void OnProjectFileRemoved(IVsProject project, string file)
        {
            Logger.Debug("Project file removed: {0}", file);
            if (TestAdapterInfo.IsTestContainer(file) || Containers.Any(c => c.HasFile(file)))
            {
                Containers.CreateContainer(CreateTestContainerSource(project, file));
            }
        }

        private void OnProjectFileRenamed(IVsProject project, string oldFile, string newFile)
        {
            OnProjectFileRemoved(project, oldFile);
            OnProjectFileAdded(project, newFile);
        }

        private bool _shouldRefresh = false;
        private object _refreshLock = new object();
        public void RefreshTestContainers(string reason = null)
        {
            if (!_initialContainerSearch)
            {
                lock (_refreshLock)
                {
                    if (!_shouldRefresh)
                    {
                        _shouldRefresh = true;
                        if (!string.IsNullOrWhiteSpace(reason))
                        {
                            Logger.Info("Refreshing containers: {0}", reason);
                        }
                    }
                }
                TPL.Task.Delay(500).ContinueWith(t =>
                {
                    if (_shouldRefresh)
                    {
                        Containers.CreateContainers(FindMissingSources());
                    }
                    lock (_refreshLock)
                    {
                        if (_shouldRefresh)
                        {
                            _shouldRefresh = false;
                            OnTestContainersChanged();
                        }
                    }
                });
            }
        }

        public IEnumerable<TestContainerSource> FindMissingSources()
        {
            return FindSources()
                .Where(s => !Containers.Any(c => c.HasFile(s.Source)))
                .ToList();
        }

        public IEnumerable<TestContainerSource> FindSources()
        {
            return ServiceProvider
                .GetLoadedProjects()
                .SelectMany(p => FindSources(p));
        }

        public IEnumerable<TestContainerSource> FindSources(IVsProject project)
        {
            Logger.Debug("Finding sources for {0}", project.GetProjectName());

            var containers = project.GetProjectItems()
                .Where(f => TestAdapterInfo.IsTestContainer(f) && File.Exists(f))
                .Select(f => new { Path = f, Directory = Path.GetDirectoryName(f), Priority = TestAdapterInfo.GetContainerPriority(f) })
                .ToList();

            return from c in containers
                   where !containers.Any(d => PathUtils.PathsEqual(c.Directory, d.Directory) && d.Priority > c.Priority)
                   select CreateTestContainerSource(project, c.Path);
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        // Flag: Has Dispose already been called? 
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            Logger.Info("Disposing of TestContainerDiscoverer");

            if (disposing)
            {
                if (Containers != null)
                {
                    Containers.Dispose();
                    Containers = null;
                }

                if (ProjectListener != null)
                {
                    ProjectListener.Dispose();
                    ProjectListener = null;
                }

                if (SolutionListener != null)
                {
                    SolutionListener.Dispose();
                    SolutionListener = null;
                }
            }
        }

        ~TestContainerDiscoverer()
        {
            Dispose(false);
        }

        #endregion
    }
}