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
        private ITestFilesUpdateWatcher _testFilesUpdateWatcher;
        private ITestFileAddRemoveListener _testFilesAddRemoveListener;
        private bool _initialContainerSearch = true;
        private List<KarmaTestContainer> _cachedContainers = new List<KarmaTestContainer>();
        public IKarmaLogger Logger { get; set; }
        private readonly HashSet<string> _files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        [ImportingConstructor]
        public KarmaTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            ILogger logger,
            ISolutionListener solutionListener,
            ITestFilesUpdateWatcher testFilesUpdateWatcher,
            ITestFileAddRemoveListener testFilesAddRemoveListener)
        {
            Logger = KarmaLogger.Create(logger: logger);

            _serviceProvider = serviceProvider;
            _solutionListener = solutionListener;

            _testFilesUpdateWatcher = testFilesUpdateWatcher;
            _testFilesUpdateWatcher.Changed += OnProjectItemChanged;

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
                    _cachedContainers.Clear();
                    WatchProjectDirectories();
                    AddFiles(FindTestFiles());
                    _initialContainerSearch = false;
                }
                return _cachedContainers;
            }
        }

        private void WatchProjectDirectories()
        {
            foreach (var project in GetProjects())
            {
                try
                {
                    _testFilesUpdateWatcher.AddDirectory(project.GetProjectDirectory());
                }
                catch (ArgumentNullException)
                {
                    // do nothing
                }
            }
        }

        private bool _shouldRefresh = false;
        private object _refreshLock = new object();
        private async void RefreshTestContainers(string reason)
        {
            if (!_initialContainerSearch)
            {
                lock (_refreshLock)
                {
                    _shouldRefresh = true;
                    Logger.Info(reason);
                }
                await Tasks.Task.Delay(TimeSpan.FromMilliseconds(2000));
                lock (_refreshLock)
                {
                    if (_shouldRefresh)
                    {
                        _shouldRefresh = false;
                        _cachedContainers = _cachedContainers.Select(c => c.Refresh()).ToList();
                        OnTestContainersChanged();
                    }
                }
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
            _initialContainerSearch = true;
        }

        private void SolutionListenerOnSolutionUnloaded(object sender, EventArgs eventArgs)
        {
            DoNotRefreshTestContainers();
            _testFilesUpdateWatcher.Clear();
            _cachedContainers.Clear();
            _initialContainerSearch = true;
        }

        private void OnSolutionProjectChanged(object sender, SolutionListenerEventArgs e)
        {
            if (e != null)
            {
                if (e.ChangedReason == SolutionChangedReason.Load)
                {
                    _testFilesUpdateWatcher.AddDirectory(e.Project.GetProjectDirectory());
                    AddFiles(FindTestFiles(e.Project));
                    RefreshTestContainers("Solution loaded");
                }
                else if (e.ChangedReason == SolutionChangedReason.Unload)
                {
                    _testFilesUpdateWatcher.RemoveDirectory(e.Project.GetProjectDirectory());
                    RemoveFiles(FindTestFiles(e.Project));
                    RefreshTestContainers("Solution unloaded");
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
                _testFilesUpdateWatcher.AddWatch(file);
                AddTestContainerIfTestFile(file);
            }
        }

        private void RemoveFiles(IEnumerable<string> files)
        {
            _files.Remove(files);
            foreach (var file in files)
            {
                _testFilesUpdateWatcher.RemoveWatch(file);
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
                            _files.Add(e.File);
                            _testFilesUpdateWatcher.AddWatch(e.File);
                            if (!AddTestContainerIfTestFile(e.File) && _cachedContainers.Count(c => c.FileAdded(e.File)) > 0)
                            {
                                RefreshTestContainers(string.Format("File added: {0}", e.File));
                            }
                            break;
                        case TestFileChangedReason.Removed:
                            _files.Remove(e.File);
                            _testFilesUpdateWatcher.RemoveWatch(e.File);
                            if (!RemoveTestContainer(e.File) && _cachedContainers.Count(c => c.FileRemoved(e.File)) > 0)
                            {
                                RefreshTestContainers(string.Format("File removed: {0}", e.File));
                            }
                            break;
                        case TestFileChangedReason.Changed:
                        case TestFileChangedReason.Saved:
                            if (!AddTestContainerIfTestFile(e.File) && _cachedContainers.Count(c => c.FileChanged(e.File)) > 0)
                            {
                                RefreshTestContainers(string.Format("File changed: {0}", e.File));
                            }
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
                Logger.Info("OnProjectItemChanged: <unknown>");
            }
        }

        private bool AddTestContainerIfTestFile(string file)
        {
            var directory = Path.GetDirectoryName(file);
            var settingsFilename = Path.Combine(directory, Globals.SettingsFilename);
            var container = "";
            if (PathUtils.PathsEqual(file, settingsFilename) || PathUtils.PathHasFileName(file, Globals.KarmaSettingsFilename) && !_files.Contains(settingsFilename))
            {
                container = file;
            }
            if (!string.IsNullOrWhiteSpace(container))
            {
                RemoveTestContainersInDirectory(directory);
                if (File.Exists(container))
                {
                    _cachedContainers.Add(new KarmaTestContainer(this, container, Logger));
                }
                RefreshTestContainers(string.Format("Test container added: {0}", file));
                return true;
            }
            return false;
        }

        private bool RemoveTestContainersInDirectory(string directory)
        {
            return RemoveTestContainers(_cachedContainers.Where(c => PathUtils.IsInDirectory(c.Source, directory)));
        }

        private bool RemoveTestContainer(string file)
        {
            var result = RemoveTestContainers(_cachedContainers.Where(c => PathUtils.PathsEqual(c.Source, file)));
            var directory = Path.GetDirectoryName(file);
            var defaultKarmaSettingsFilename = Path.Combine(directory, Globals.SettingsFilename);
            if (PathUtils.PathHasFileName(file, Globals.SettingsFilename) && _files.Contains(defaultKarmaSettingsFilename))
            {
                AddTestContainerIfTestFile(defaultKarmaSettingsFilename);
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
                    _cachedContainers.Remove(container);
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
            return project.GetProjectItems().Where(f => File.Exists(f));
        }

        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_testFilesUpdateWatcher != null)
                {
                    _testFilesUpdateWatcher.Changed -= OnProjectItemChanged;
                    _testFilesUpdateWatcher.Dispose();
                    _testFilesUpdateWatcher = null;
                }

                if (_testFilesAddRemoveListener != null)
                {
                    _testFilesAddRemoveListener.Changed -= OnProjectItemChanged;
                    _testFilesAddRemoveListener.StopListening();
                    _testFilesAddRemoveListener.Dispose();
                    _testFilesAddRemoveListener = null;
                }

                if (_solutionListener != null)
                {
                    _solutionListener.ProjectChanged -= OnSolutionProjectChanged;
                    _solutionListener.StopListening();
                    _solutionListener.Dispose();
                    _solutionListener = null;
                }
            }
        }
    }
}
