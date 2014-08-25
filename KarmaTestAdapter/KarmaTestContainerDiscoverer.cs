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
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class KarmaTestContainerDiscoverer : ITestContainerDiscoverer
    {
        private IServiceProvider _serviceProvider;
        private ISolutionEventsListener _solutionListener;
        private ITestFilesUpdateWatcher _testFilesUpdateWatcher;
        private ITestFileAddRemoveListener _testFilesAddRemoveListener;
        private bool _initialContainerSearch = true;
        private bool _shouldRefresh = false;
        private List<KarmaTestContainer> _cachedContainers = new List<KarmaTestContainer>();
        private IKarmaLogger _logger;
        private readonly HashSet<string> _files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        [ImportingConstructor]
        public KarmaTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            ILogger logger,
            ISolutionEventsListener solutionListener,
            ITestFilesUpdateWatcher testFilesUpdateWatcher,
            ITestFileAddRemoveListener testFilesAddRemoveListener)
        {
            _logger = KarmaLogger.Create(logger: logger);

            _serviceProvider = serviceProvider;
            _solutionListener = solutionListener;

            _testFilesUpdateWatcher = testFilesUpdateWatcher;
            _testFilesUpdateWatcher.FileChangedEvent += OnProjectItemChanged;

            _testFilesAddRemoveListener = testFilesAddRemoveListener;
            _testFilesAddRemoveListener.TestFileChanged += OnProjectItemChanged;
            _testFilesAddRemoveListener.StartListeningForTestFileChanges();

            _solutionListener.SolutionUnloaded += SolutionListenerOnSolutionUnloaded;
            _solutionListener.SolutionProjectChanged += OnSolutionProjectChanged;
            _solutionListener.StartListeningForChanges();
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
                    AddFiles(FindTestFiles());
                    _initialContainerSearch = false;
                }
                else if (_shouldRefresh)
                {
                    _shouldRefresh = false;
                    _cachedContainers = _cachedContainers.Select(c => c.FreshCopy()).ToList();
                }
                return _cachedContainers;
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
            _initialContainerSearch = true;
            _shouldRefresh = false;
        }

        private void SolutionListenerOnSolutionUnloaded(object sender, EventArgs eventArgs)
        {
            _initialContainerSearch = true;
            _shouldRefresh = false;
            _cachedContainers.Clear();
        }

        private void OnSolutionProjectChanged(object sender, SolutionEventsListenerEventArgs e)
        {
            if (e != null)
            {
                if (e.ChangedReason == SolutionChangedReason.Load)
                {
                    AddFiles(FindTestFiles(e.Project));
                    _shouldRefresh = true;
                }
                else if (e.ChangedReason == SolutionChangedReason.Unload)
                {
                    RemoveFiles(FindTestFiles(e.Project));
                    _shouldRefresh = true;
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
                switch (e.ChangedReason)
                {
                    case TestFileChangedReason.Added:
                        _files.Add(e.File);
                        _testFilesUpdateWatcher.AddWatch(e.File);
                        AddTestContainerIfTestFile(e.File);
                        break;
                    case TestFileChangedReason.Removed:
                        _files.Remove(e.File);
                        _testFilesUpdateWatcher.RemoveWatch(e.File);
                        RemoveTestContainer(e.File);
                        break;
                    case TestFileChangedReason.Changed:
                        AddTestContainerIfTestFile(e.File);
                        break;
                }

                OnTestContainersChanged();
            }
        }

        private void AddTestContainerIfTestFile(string file)
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
                    _cachedContainers.Add(new KarmaTestContainer(this, container));
                }
            }
            _shouldRefresh = true;
        }

        private void RemoveTestContainersInDirectory(string directory)
        {
            _cachedContainers.RemoveAll(c => PathUtils.IsInDirectory(c.Source, directory));
            _shouldRefresh = true;
        }

        private void RemoveTestContainer(string file)
        {
            _cachedContainers.RemoveAll(c => PathUtils.PathsEqual(c.Source, file));
            var directory = Path.GetDirectoryName(file);
            var defaultKarmaSettingsFilename = Path.Combine(directory, Globals.SettingsFilename);
            if (PathUtils.PathHasFileName(file, Globals.SettingsFilename) && _files.Contains(defaultKarmaSettingsFilename))
            {
                AddTestContainerIfTestFile(defaultKarmaSettingsFilename);
            }
            _shouldRefresh = true;
        }

        private IEnumerable<string> FindTestFiles()
        {
            var solution = (IVsSolution)_serviceProvider.GetService(typeof(SVsSolution));
            var loadedProjects = solution.EnumerateLoadedProjects(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION).OfType<IVsProject>();

            return loadedProjects.SelectMany(p => FindTestFiles(p));
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
                    _testFilesUpdateWatcher.FileChangedEvent -= OnProjectItemChanged;
                    ((IDisposable)_testFilesUpdateWatcher).Dispose();
                    _testFilesUpdateWatcher = null;
                }

                if (_testFilesAddRemoveListener != null)
                {
                    _testFilesAddRemoveListener.TestFileChanged -= OnProjectItemChanged;
                    _testFilesAddRemoveListener.StopListeningForTestFileChanges();
                    _testFilesAddRemoveListener = null;
                }

                if (_solutionListener != null)
                {
                    _solutionListener.SolutionProjectChanged -= OnSolutionProjectChanged;
                    _solutionListener.StopListeningForChanges();
                    _solutionListener = null;
                }
            }
        }
    }
}
