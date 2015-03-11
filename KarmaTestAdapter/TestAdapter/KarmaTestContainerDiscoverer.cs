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
            SolutionListener solutionListener,
            KarmaTestSettingsProvider testSettingsService,
            ILogger logger
            )
        {
            Logger = new KarmaLogger(logger, "ContainerDiscoverer", true);
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
        }

        private bool _initialContainerSearch = true;
        private ITestsService _testsService;

        public KarmaTestContainerList Containers { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        public SolutionListener SolutionListener { get; private set; }
        public IKarmaLogger Logger { get; set; }
        public KarmaTestSettingsProvider TestSettingsProvider { get; private set; }
        public KarmaTestSettings TestSettings { get { return TestSettingsProvider.Settings; } }
        public string BaseDirectory { get { return ServiceProvider.GetSolutionDirectory(); } }

        public Uri ExecutorUri
        {
            get { return Globals.ExecutorUri; }
        }

        public void RunTests(IEnumerable<Guid> tests)
        {
            if (_testsService != null)
            {
                _testsService.RunTestsAsync(tests);
            }
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
                if (e.ChangedReason == SolutionChangedReason.Load)
                {
                    Containers.Clear();
                    Containers.CreateContainers(FindSources());
                    RefreshTestContainers("Project loaded");
                }
                else if (e.ChangedReason == SolutionChangedReason.Unload)
                {
                    Containers.Clear();
                    Containers.CreateContainers(FindSources());
                    RefreshTestContainers("Project unloaded");
                }
            }
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

        private IEnumerable<string> FindSources()
        {
            var containers = ServiceProvider
                .GetLoadedProjects()
                .SelectMany(p => FindSources(p))
                .Select(f => PathUtils.GetPhysicalPath(f))
                .ToList();
            return containers;
        }

        private IEnumerable<string> FindSources(IVsProject project)
        {
            var containers = project
                .GetProjectItems()
                .Where(f => PathUtils.PathHasFileName(f, Globals.SettingsFilename) || PathUtils.PathHasFileName(f, Globals.KarmaConfigFilename))
                .Where(f => File.Exists(f))
                .ToList();

            return from c in containers
                   let s = Path.Combine(Path.GetDirectoryName(c), Globals.SettingsFilename)
                   where PathUtils.PathsEqual(c, s) || !containers.Any(d => PathUtils.PathsEqual(d, s))
                   select c;
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