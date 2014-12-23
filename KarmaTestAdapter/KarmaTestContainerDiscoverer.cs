using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tasks = System.Threading.Tasks;

namespace KarmaTestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class KarmaTestContainerDiscoverer : ITestContainerDiscoverer
    {
        private IServiceProvider _serviceProvider;
        private ISolutionListener _solutionListener;
        private ITestFileAddRemoveListener _testFilesAddRemoveListener;
        private bool _initialContainerSearch = true;
        private KarmaTestContainerList _containers;
        public IKarmaLogger Logger { get; set; }
        private readonly HashSet<string> _files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        [ImportingConstructor]
        public KarmaTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            ILogger logger,
            ISolutionListener solutionListener,
            ITestFileAddRemoveListener testFilesAddRemoveListener)
        {
            Logger = KarmaLogger.Create(logger: logger);

            _serviceProvider = serviceProvider;
            _solutionListener = solutionListener;
            _containers = new KarmaTestContainerList(this);

            _testFilesAddRemoveListener = testFilesAddRemoveListener;
            _testFilesAddRemoveListener.Changed += OnProjectItemChanged;
            _testFilesAddRemoveListener.StartListening();

            _solutionListener.SolutionUnloaded += SolutionListenerOnSolutionUnloaded;
            _solutionListener.ProjectChanged += OnSolutionProjectChanged;
            _solutionListener.StartListening();

            Logger.Info("KarmaTestContainerDiscoverer created");
        }

        public event EventHandler TestContainersUpdated;

        public Uri ExecutorUri
        {
            get { return Globals.ExecutorUri; }
        }

        public IEnumerable<ITestContainer> TestContainers
        {
            get
            {
                if (_initialContainerSearch)
                {
                    _containers.Clear();
                    AddFiles(FindTestFiles());
                    _initialContainerSearch = false;
                }
                return _containers;
            }
        }

        private string _baseDirectory = null;
        public string BaseDirectory
        {
            get
            {
                if (_baseDirectory == null)
                {
                    var solution = (IVsSolution)_serviceProvider.GetService(typeof(SVsSolution));
                    _baseDirectory = solution.GetSolutionDirectory();
                }
                return _baseDirectory ?? null;
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
                    Logger.Info(reason);
                }
                Tasks.Task.Delay(TimeSpan.FromMilliseconds(500)).ContinueWith(t => {
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

        private void DoNotRefreshTestContainers()
        {
            lock (_refreshLock)
            {
                _shouldRefresh = false;
            }
        }

        private void OnTestContainersChanged()
        {
            if (TestContainersUpdated != null && !_initialContainerSearch)
            {
                TestContainersUpdated(this, EventArgs.Empty);
            }
        }

        private void SolutionListenerOnSolutionLoaded(object sender, EventArgs eventArgs)
        {
            DoNotRefreshTestContainers();
            _baseDirectory = null;
            _initialContainerSearch = true;
        }

        private void SolutionListenerOnSolutionUnloaded(object sender, EventArgs eventArgs)
        {
            DoNotRefreshTestContainers();
            _baseDirectory = null;
            _containers.Clear();
            _initialContainerSearch = true;
        }

        private void OnSolutionProjectChanged(object sender, SolutionListenerEventArgs e)
        {
            if (e != null)
            {
                if (e.ChangedReason == SolutionChangedReason.Load)
                {
                    AddFiles(FindTestFiles(e.Project));
                    RefreshTestContainers("Project loaded");
                }
                else if (e.ChangedReason == SolutionChangedReason.Unload)
                {
                    RemoveFiles(FindTestFiles(e.Project));
                    RefreshTestContainers("Project unloaded");
                }
            }

            // Do not fire OnTestContainersChanged here.
            // This will cause us to fire this event too early before the UTE is ready to process containers and will result in an exception.
            // The UTE will query all the TestContainerDiscoverers once the solution is loaded.
        }

        private void AddFiles(IEnumerable<string> files)
        {
            _files.Add(files);
            foreach (var file in files)
            {
                AddTestContainerIfTestFile(file);
            }
        }

        private void RemoveFiles(IEnumerable<string> files)
        {
            _files.Remove(files);
            foreach (var file in files)
            {
                RemoveTestContainer(file);
            }
        }

        private void OnProjectItemChanged(object sender, TestFileChangedEventArgs e)
        {
            if (e != null)
            {
                try
                {
                    switch (e.ChangedReason)
                    {
                        case TestFileChangedReason.Added:
                            AddTestContainerIfTestFile(e.File);
                            break;
                        case TestFileChangedReason.Removed:
                            RemoveTestContainer(e.File);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            else
            {
                Logger.Error("OnProjectItemChanged: <unknown>");
            }
        }

        public bool AddTestContainerIfTestFile(string file)
        {
            var directory = Path.GetDirectoryName(file);
            var settingsFile = Path.Combine(directory, Globals.SettingsFilename);
            var karmaConfigFile = Path.Combine(directory, Globals.KarmaConfigFilename);
            var source = "";
            if (PathUtils.PathsEqual(file, settingsFile) || PathUtils.PathsEqual(file, karmaConfigFile) && !_files.Contains(settingsFile))
            {
                source = file;
            }
            if (!string.IsNullOrWhiteSpace(source))
            {
                RemoveTestContainersInDirectory(directory);
                if (File.Exists(source))
                {
                    _containers.CreateContainer(source);
                }
                RefreshTestContainers(string.Format("Test container added: {0}", file));
                return true;
            }
            return false;
        }

        private bool RemoveTestContainersInDirectory(string directory)
        {
            return RemoveTestContainers(_containers.Where(c => PathUtils.IsInDirectory(c.Source, directory)));
        }

        public bool RemoveTestContainer(string file)
        {
            var result = RemoveTestContainers(_containers.Where(c => PathUtils.PathsEqual(c.Source, file)));
            var directory = Path.GetDirectoryName(file);
            var settingsFilename = Path.Combine(directory, Globals.SettingsFilename);
            if (PathUtils.PathHasFileName(file, Globals.SettingsFilename) && _files.Contains(settingsFilename))
            {
                AddTestContainerIfTestFile(settingsFilename);
            }
            return result;
        }

        private bool RemoveTestContainers(IEnumerable<KarmaTestContainer> containersToRemove)
        {
            containersToRemove = containersToRemove.ToList();
            if (containersToRemove.Any())
            {
                foreach (var container in containersToRemove)
                {
                    _containers.Remove(container);
                    RefreshTestContainers(string.Format("Test container removed: {0}", container.Source));
                }
                return true;
            }
            return false;
        }

        private IEnumerable<string> FindTestFiles()
        {
            return GetProjects().SelectMany(p => FindTestFiles(p));
        }

        private IEnumerable<IVsProject> GetProjects()
        {
            var solution = (IVsSolution)_serviceProvider.GetService(typeof(SVsSolution));
            return solution.EnumerateLoadedProjects(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION).OfType<IVsProject>();
        }

        private IEnumerable<string> FindTestFiles(IVsProject project)
        {
            return project
                .GetProjectItems()
                .Where(f => PathUtils.PathHasFileName(f, Globals.SettingsFilename) || PathUtils.PathHasFileName(f, Globals.KarmaConfigFilename))
                .Where(f => File.Exists(f));
        }

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
            if (_disposed)
                return;

            if (disposing)
            {
                if (_containers != null)
                {
                    _containers.Dispose();
                    _containers = null;
                }

                if (_testFilesAddRemoveListener != null)
                {
                    _testFilesAddRemoveListener.Dispose();
                    _testFilesAddRemoveListener = null;
                }

                if (_solutionListener != null)
                {
                    _solutionListener.Dispose();
                    _solutionListener = null;
                }
            }

            _disposed = true;
        }

        ~KarmaTestContainerDiscoverer()
        {
            Dispose(false);
        }
    }
}
