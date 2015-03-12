using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
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

namespace KarmaTestAdapter.TestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class KarmaTestContainerDiscoverer : ITestContainerDiscoverer, IDisposable
    {
        [ImportingConstructor]
        public KarmaTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            ITestsService testsService,
            ISolutionListener solutionListener,
            KarmaTestSettingsProvider testSettingsService,
            ILogger logger
            )
        {
            Logger = new KarmaLogger(logger, true);
            Logger.Info("KarmaTestContainerDiscoverer created");
            TestSettingsProvider = testSettingsService;

            Containers = new KarmaTestContainerList(this);

            ServiceProvider = serviceProvider;
            _testsService = testsService;

            SolutionListener = solutionListener;
            SolutionListener.SolutionLoaded += OnSolutionLoaded;
            SolutionListener.SolutionUnloaded += OnSolutionUnloaded;
            SolutionListener.ProjectChanged += OnSolutionProjectChanged;
            SolutionListener.StartListening();

            ProjectListener = new ProjectListener(serviceProvider, Logger);
            ProjectListener.FileAdded += (source, e) => OnProjectFileAdded(e.Project, e.File);
            ProjectListener.FileRemoved += (source, e) => OnProjectFileRemoved(e.Project, e.File);
            ProjectListener.FileRenamed += (source, e) => OnProjectFileRenamed(e.Project, e.OldFile, e.NewFile);
            ProjectListener.StartListening();
        }

        private bool _initialContainerSearch = true;
        private ITestsService _testsService;

        public KarmaTestContainerList Containers { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public ISolutionListener SolutionListener { get; private set; }
        public IProjectListener ProjectListener { get; private set; }
        public IKarmaLogger Logger { get; set; }
        public KarmaTestSettingsProvider TestSettingsProvider { get; private set; }
        public KarmaTestSettings TestSettings { get { return TestSettingsProvider.Settings; } }
        public string BaseDirectory { get { return ServiceProvider.GetSolutionDirectory(); } }

        public Uri ExecutorUri
        {
            get { return Globals.ExecutorUri; }
        }

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
            Containers.Clear();
            Containers.CreateContainers(FindSources());
            _initialContainerSearch = true;
        }

        private void OnSolutionUnloaded(object sender, EventArgs e)
        {
            Logger.Info("Solution unloaded");
            Containers.Clear();
            _initialContainerSearch = true;
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
            if (PathUtils.IsSettingsFile(file) || PathUtils.IsKarmaConfigFile(file) && !Containers.Any(c => c.HasFile(file)))
            {
                Containers.CreateContainer(new KarmaTestContainerSourceInfo(project, file));
            }
        }

        private void OnProjectFileRemoved(IVsProject project, string file)
        {
            Logger.Debug("Project file removed: {0}", file);
            if (PathUtils.IsSettingsFile(file))
            {
                Containers.Remove(file);

                TPL.Task.Delay(100).ContinueWith(t =>
                {
                    var karmaConfigFile = Path.Combine(Path.GetDirectoryName(file), Globals.KarmaConfigFilename);
                    if (project.GetSources().Any(s => PathUtils.PathsEqual(s, karmaConfigFile)) && !Containers.Any(c => c.HasFile(karmaConfigFile)))
                    {
                        Containers.CreateContainer(new KarmaTestContainerSourceInfo(project, karmaConfigFile));
                    }
                });
            }
            else if (PathUtils.IsKarmaConfigFile(file))
            {
                Containers.Remove(file);
            }
        }

        private void OnProjectFileRenamed(IVsProject project, string oldFile, string newFile)
        {
            OnProjectFileRemoved(project, oldFile);
            OnProjectFileAdded(project, newFile);
        }

        private bool _shouldRefresh = false;
        private object _refreshLock = new object();
        public void RefreshTestContainers(string reason)
        {
            if (!_initialContainerSearch)
            {
                lock (_refreshLock)
                {
                    _shouldRefresh = true;
                    Logger.Info("Refreshing containers: {0}", reason);
                }
                TPL.Task.Delay(500).ContinueWith(t =>
                {
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

        private IEnumerable<KarmaTestContainerSourceInfo> FindSources()
        {
            return ServiceProvider
                .GetLoadedProjects()
                .SelectMany(p => FindSources(p));
        }

        private IEnumerable<KarmaTestContainerSourceInfo> FindSources(IVsProject project)
        {
            Logger.Debug("Finding sources for {0}", project.GetProjectName());

            var containers = project.GetSources().ToList();

            return from c in containers
                   let s = Path.Combine(Path.GetDirectoryName(c), Globals.SettingsFilename)
                   where PathUtils.PathsEqual(c, s) || !containers.Any(d => PathUtils.PathsEqual(d, s))
                   select new KarmaTestContainerSourceInfo(project, c);
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

            Logger.Info("Disposing of KarmaTestContainerDiscoverer");

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

        ~KarmaTestContainerDiscoverer()
        {
            Dispose(false);
        }

        #endregion
    }
}