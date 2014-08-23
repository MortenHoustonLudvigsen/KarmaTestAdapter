using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using KarmaTestAdapter.Logging;

namespace KarmaTestAdapter.Helpers
{
    [Export(typeof(ITestFilesUpdateWatcher))]
    public class TestFilesUpdateWatcher : IDisposable, ITestFilesUpdateWatcher
    {
        private class FileWatcherInfo
        {
            public FileWatcherInfo(FileSystemWatcher watcher)
            {
                Watcher = watcher;
                LastEventTime = DateTime.MinValue;
            }

            public FileSystemWatcher Watcher { get; set; }
            public DateTime LastEventTime { get; set; }
        }

        private IDictionary<string, FileWatcherInfo> _fileWatchers;

        public TestFilesUpdateWatcher()
        {
            _fileWatchers = new Dictionary<string, FileWatcherInfo>(StringComparer.OrdinalIgnoreCase);
        }

        public event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        public void AddWatch(string path)
        {
            ValidateArg.NotNullOrEmpty(path, "path");

            if (!String.IsNullOrEmpty(path))
            {
                var directoryName = Path.GetDirectoryName(path);
                var fileName = Path.GetFileName(path);

                FileWatcherInfo watcherInfo;
                if (!_fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    watcherInfo = new FileWatcherInfo(new FileSystemWatcher(directoryName, fileName));
                    _fileWatchers.Add(path, watcherInfo);

                    watcherInfo.Watcher.Changed += OnChanged;
                    watcherInfo.Watcher.EnableRaisingEvents = true;
                }
            }
        }

        public void RemoveWatch(string path)
        {
            ValidateArg.NotNullOrEmpty(path, "path");

            if (!String.IsNullOrEmpty(path))
            {
                FileWatcherInfo watcherInfo;
                if (_fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    watcherInfo.Watcher.EnableRaisingEvents = false;

                    _fileWatchers.Remove(path);

                    watcherInfo.Watcher.Changed -= OnChanged;
                    watcherInfo.Watcher.Dispose();
                    watcherInfo.Watcher = null;
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (FileChangedEvent != null)
            {
                FileWatcherInfo watcherInfo;
                if (_fileWatchers.TryGetValue(e.FullPath, out watcherInfo))
                {
                    var writeTime = File.GetLastWriteTime(e.FullPath);
                    // Only fire update if enough time has passed since last update to prevent duplicate events
                    if (writeTime.Subtract(watcherInfo.LastEventTime).TotalMilliseconds > 500)
                    {
                        watcherInfo.LastEventTime = writeTime;
                        FileChangedEvent(sender, new TestFileChangedEventArgs(e.FullPath, TestFileChangedReason.Changed));
                    }
                }
            }
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
            if (disposing && _fileWatchers != null)
            {
                foreach (var fileWatcher in _fileWatchers.Values)
                {
                    if (fileWatcher != null && fileWatcher.Watcher != null)
                    {
                        fileWatcher.Watcher.Changed -= OnChanged;
                        fileWatcher.Watcher.Dispose();
                    }
                }

                _fileWatchers.Clear();
                _fileWatchers = null;
            }
        }
    }
}