using KarmaTestAdapter.Commands;
using KarmaTestAdapter.Config;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public class KarmaTestContainer : KarmaTestContainerBase
    {
        private KarmaTestContainerList _containerList;
        private readonly FileInfoList _files;
        private IEnumerable<KarmaFileWatcher> _fileWatchers;
        private KarmaServeCommand _serveCommand;

        public KarmaTestContainer(KarmaTestContainerList containerList, string source, IKarmaLogger logger)
            : base(containerList.Discoverer, source, DateTime.Now)
        {
            _containerList = containerList;
            Logger = logger;
            try
            {
                Settings = new KarmaSettings(Source, Logger);
                IsValid = Settings.AreValid;
            }
            catch (Exception ex)
            {
                IsValid = false;
                logger.Error(ex, string.Format("Could not load tests from {0}", source));
            }
            if (IsValid)
            {
                TestFiles = Settings.TestFilesSpec ?? KarmaGetConfigCommand.GetConfig(Source, Logger) ?? new FilesSpec();
            }
            else
            {
                TestFiles = new FilesSpec();
            }
            _files = new FileInfoList(this, TestFiles);
            _files.Add(Source);
            if (IsValid)
            {
                _files.Add(Settings.SettingsFile);
                _files.Add(Settings.KarmaConfigFile);
            }
            _fileWatchers = GetFileWatchers().Where(w => w != null).ToList();
            StartKarmaServer();
        }

        public bool IsValid { get; private set; }
        public IKarmaLogger Logger { get; private set; }
        public KarmaSettings Settings { get; private set; }
        public Uri ExecutorUri { get { return Globals.ExecutorUri; } }
        public Karma Karma { get; set; }
        public FilesSpec TestFiles { get; private set; }

        private Dictionary<string, FileInfo> GetFiles()
        {
            var files = TestFiles ?? Enumerable.Empty<string>();
            return files.ToDictionary(f => f, f => new FileInfo(this, f), StringComparer.OrdinalIgnoreCase);
        }

        private void StartKarmaServer()
        {
            if (IsValid && Settings.ServerModeValid && !_disposed)
            {
                _serveCommand = _serveCommand ?? new KarmaServeCommand(Source);
                _serveCommand.Start(Logger, () =>
                {
                    Task.Delay(250).ContinueWith(t => StartKarmaServer());
                });
            }
        }

        private IEnumerable<KarmaFileWatcher> GetFileWatchers()
        {
            if (IsValid)
            {
                yield return CreateFileWatcher(Settings.SettingsFile);
                yield return CreateFileWatcher(Settings.KarmaConfigFile);
                foreach (var filter in TestFiles.Included.GroupBy(f => f.FileFilter, StringComparer.OrdinalIgnoreCase))
                {
                    var dirs = filter.Select(f => f.Directory);
                    foreach (var dir in dirs.Where(d1 => !dirs.Any(d2 => !string.Equals(d1, d2, StringComparison.OrdinalIgnoreCase) && d1.StartsWith(d2, StringComparison.OrdinalIgnoreCase))).Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        yield return CreateFileWatcher(dir, filter.Key, true);
                    }
                }
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
            Logger.Info(@"Watching '{0}'", PathUtils.GetRelativePath(BaseDirectory, watcher.Watching, true));
            return watcher;
        }

        private void FileWatcherChanged(object sender, TestFileChangedEventArgs e)
        {
            switch (e.ChangedReason)
            {
                case TestFileChangedReason.Added:
                    FileAdded(e.File);
                    break;
                case TestFileChangedReason.Changed:
                case TestFileChangedReason.Saved:
                    FileChanged(e.File);
                    break;
                case TestFileChangedReason.Removed:
                    FileRemoved(e.File);
                    break;
            }
        }

        public bool FileAdded(string file)
        {
            return FileChanged(file, string.Format("File added:   {0}", file), f => _files.GetHasChanges(f) || true);
        }

        public bool FileChanged(string file)
        {
            return FileChanged(file, string.Format("File changed: {0}", file), f => _files.GetHasChanges(f));
        }

        public bool FileRemoved(string file)
        {
            return FileChanged(file, string.Format("File removed: {0}", file), f => _files.Remove(f) || true);
        }

        private object _fileChangeLock = new object();
        private bool FileChanged(string file, string reason, Func<string, bool> hasChanged)
        {
            lock (_fileChangeLock)
            {
                if (KnowsFile(file))
                {
                    // The file belongs to this container
                    if (hasChanged(file))
                    {
                        TimeStamp = DateTime.Now;
                        if (IsContainer(file))
                        {
                            if (System.IO.File.Exists(Source))
                            {
                                KarmaTestContainerDiscoverer.AddTestContainerIfTestFile(Source);
                            }
                            else
                            {
                                KarmaTestContainerDiscoverer.RemoveTestContainer(Source);
                            }
                        }
                        else
                        {
                            KarmaTestContainerDiscoverer.RefreshTestContainers(reason);
                        }
                        return true;
                    }
                }
                return false;
            }
        }

        private bool IsContainer(string file)
        {
            return PathUtils.PathsEqual(file, Source)
                || PathUtils.PathsEqual(file, Settings.KarmaConfigFile)
                || PathUtils.PathsEqual(file, Settings.SettingsFile);
        }

        private bool KnowsFile(string file)
        {
            return _files.Contains(file)
                || TestFiles.Contains(file)
                || IsContainer(file);
        }

        public override string ToString()
        {
            return this.ExecutorUri.ToString() + "/" + this.Source;
        }

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_serveCommand != null)
                {
                    _serveCommand.Dispose();
                    _serveCommand = null;
                }

                if (_fileWatchers != null)
                {
                    foreach (var watcher in _fileWatchers)
                    {
                        Logger.Info(@"Stop watching '{0}'", PathUtils.GetRelativePath(BaseDirectory, watcher.Watching, true));
                        watcher.Dispose();
                    }
                    _fileWatchers = null;
                }

                if (Settings != null)
                {
                    Settings.Dispose();
                    Settings = null;
                }
            }

            _disposed = true;
        }

        private class FileInfoList
        {
            public FileInfoList(KarmaTestContainer container, IEnumerable<string> files)
            {
                Container = container;
                files = files ?? Enumerable.Empty<string>();
                _files = files.ToDictionary(f => f, f => new FileInfo(Container, f), StringComparer.OrdinalIgnoreCase);
            }

            private readonly Dictionary<string, FileInfo> _files = new Dictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);
            public KarmaTestContainer Container { get; private set; }

            public bool Contains(string file)
            {
                return _files.ContainsKey(file);
            }

            public FileInfo Add(string file)
            {
                FileInfo fileInfo;
                if (!_files.TryGetValue(file, out fileInfo))
                {
                    fileInfo = new FileInfo(Container, file);
                    _files.Add(file, fileInfo);
                }
                return fileInfo;
            }

            public bool Remove(string file)
            {
                return _files.Remove(file);
            }

            public bool GetHasChanges(string file)
            {
                if (!string.IsNullOrWhiteSpace(file))
                {
                    FileInfo fileInfo;
                    if (_files.TryGetValue(file, out fileInfo))
                    {
                        var result = fileInfo.Update();
                        if (!fileInfo.Exists)
                        {
                            Remove(file);
                        }
                        return result;
                    }
                    Add(file);
                    return true;
                }
                return false;
            }
        }

        private class FileInfo
        {
            public FileInfo(KarmaTestContainer container, string path)
            {
                Container = container;
                Path = path;
                Update();
            }

            public KarmaTestContainer Container { get; private set; }
            public string Path { get; private set; }
            public bool Exists { get; private set; }
            public string Hash { get; private set; }

            public bool Update()
            {
                var exists = System.IO.File.Exists(Path);
                var hash = exists ? GetHash() : null;
                if (exists != Exists || hash != Hash || hash == null)
                {
                    Exists = exists;
                    Hash = hash;
                    return true;
                }
                return false;
            }

            private string GetHash()
            {
                try
                {
                    return Sha1Utils.GetHash(Path);
                }
                catch (Exception ex)
                {
                    Container.Logger.Warn("Could not get hash for file {0}: {1}", PathUtils.GetRelativePath(Container.BaseDirectory, Path, true), ex.Message);
                    return null;
                }
            }
        }
    }
}
