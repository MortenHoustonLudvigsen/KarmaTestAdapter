using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using KarmaTestAdapter.Logging;

namespace KarmaTestAdapter.Helpers
{
    [Export(typeof(ITestFilesUpdateWatcher))]
    public class TestFilesUpdateWatcher : ITestFilesUpdateWatcher, IDisposable
    {
        private class DirectoryWatchers : IDisposable
        {
            private readonly IDictionary<string, DirectoryWatcher> _dictionaryWatchers = new Dictionary<string, DirectoryWatcher>(StringComparer.OrdinalIgnoreCase);
            private object _lock = new object();

            public event EventHandler<TestFileChangedEventArgs> Changed;
            private void OnChanged(object sender, TestFileChangedEventArgs e)
            {
                if (Changed != null)
                {
                    Changed(this, e);
                }
            }

            public DirectoryWatcher AddDirectory(string fullPath)
            {
                return Get(fullPath, true, w => w);
            }

            public bool RemoveDirectory(string fullPath)
            {
                return Get(fullPath, false, w =>
                {
                    _dictionaryWatchers.Remove(w.FullPath);
                    w.Changed -= OnChanged;
                    w.Dispose();
                    return true;
                });
            }

            public void Clear()
            {
                lock (_lock)
                {
                    foreach (var watcher in _dictionaryWatchers.Values)
                    {
                        watcher.Changed -= OnChanged;
                        watcher.Dispose();
                    }
                    _dictionaryWatchers.Clear();
                }
            }

            private T Get<T>(string fullPath, bool createIfNotExists, Func<DirectoryWatcher, T> action)
            {
                lock (_lock)
                {
                    DirectoryWatcher watcher;
                    if (_dictionaryWatchers.TryGetValue(fullPath, out watcher))
                    {
                        return action(watcher);
                    }
                    if (createIfNotExists)
                    {
                        watcher = new DirectoryWatcher(fullPath);
                        watcher.Changed += OnChanged;
                        _dictionaryWatchers.Add(watcher.FullPath, watcher);
                        return action(watcher);
                    }
                    return default(T);
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
                if (disposing)
                {
                    Clear();
                }
            }
        }

        private class DirectoryWatcher : IDisposable
        {
            public DirectoryWatcher(string fullPath)
            {
                FullPath = fullPath;
                Watcher = new FileSystemWatcher(FullPath)
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    InternalBufferSize = 64 * 1024,
                    IncludeSubdirectories = true
                };
                Watcher.Changed += OnChanged;
                Watcher.EnableRaisingEvents = true;
            }

            public event EventHandler<TestFileChangedEventArgs> Changed;
            void OnChanged(object sender, FileSystemEventArgs e)
            {
                if (Changed != null)
                {
                    Changed(sender, new TestFileChangedEventArgs(e.FullPath, TestFileChangedReason.Changed));
                }
            }

            public FileSystemWatcher Watcher { get; private set; }
            public string FullPath { get; private set; }

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
                    if (Watcher != null)
                    {
                        Watcher.Changed -= OnChanged;
                        Watcher.Dispose();
                        Watcher = null;
                    }
                }
            }
        }

        private DirectoryWatchers _directoryWatchers = new DirectoryWatchers();
        private HashSet<string> _files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public TestFilesUpdateWatcher()
        {
            _directoryWatchers.Changed += OnChanged;
        }

        public event EventHandler<TestFileChangedEventArgs> Changed;
        void OnChanged(object sender, TestFileChangedEventArgs e)
        {
            if (Changed != null && _files.Contains(e.File))
            {
                Changed(sender, e);
            }
        }

        public void AddDirectory(string path)
        {
            _directoryWatchers.AddDirectory(path);
        }

        public void RemoveDirectory(string path)
        {
            _directoryWatchers.RemoveDirectory(path);
        }

        public void Clear()
        {
            _directoryWatchers.Clear();
        }

        public void AddWatch(string path)
        {
            _files.Add(path);
        }

        public void RemoveWatch(string path)
        {
            _files.Remove(path);
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
                _files.Clear();
                if (_directoryWatchers != null)
                {
                    _directoryWatchers.Changed -= OnChanged;
                    _directoryWatchers.Dispose();
                    _directoryWatchers = null;
                }
            }
        }
    }
}