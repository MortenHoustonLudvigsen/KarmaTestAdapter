using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JsTestAdapter.Helpers
{
    public enum FileChangedReason
    {
        None,
        Added,
        Removed,
        Changed
    }

    public class FileChangedEventArgs : System.EventArgs
    {
        public string File { get; private set; }
        public FileChangedReason ChangedReason { get; private set; }

        public FileChangedEventArgs(string file, FileChangedReason reason)
        {
            File = file;
            ChangedReason = reason;
        }
    }

    public class FileWatcher : IDisposable
    {
        public FileWatcher(string directory, string filter, bool includeSubdirectories = true)
        {
            Directory = directory;
            Filter = filter;
            _watcher = new FileSystemWatcher
            {
                Path = Directory,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.CreationTime,
                Filter = Filter,
                IncludeSubdirectories = includeSubdirectories
            };

            _watcher.Created += new FileSystemEventHandler(OnCreated);
            _watcher.Changed += new FileSystemEventHandler(OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(OnDeleted);
            _watcher.Renamed += new RenamedEventHandler(OnRenamed);

            _watcher.EnableRaisingEvents = true;
        }

        private FileSystemWatcher _watcher;
        public string Directory { get; private set; }
        public string Filter { get; private set; }
        public bool IncludeSubdirectories { get { return _watcher.IncludeSubdirectories; } }
        public string Watching { get { return IncludeSubdirectories ? string.Format(@"{0}\**\{1}", Directory, Filter) : string.Format(@"{0}\{1}", Directory, Filter); } }

        private class EventTransition
        {
            public EventTransition(FileChangedReason previousReason, FileChangedReason currentReason, FileChangedReason newReason)
            {
                PreviousReason = previousReason;
                CurrentReason = currentReason;
                NewReason = newReason;
            }

            public FileChangedReason PreviousReason { get; private set; }
            public FileChangedReason CurrentReason { get; private set; }
            public FileChangedReason NewReason { get; private set; }
        }

        private static readonly List<EventTransition> _eventTransitions = new List<EventTransition>
        {
            new EventTransition(FileChangedReason.None, FileChangedReason.None, FileChangedReason.None),
            new EventTransition(FileChangedReason.None, FileChangedReason.Added, FileChangedReason.Added),
            new EventTransition(FileChangedReason.None, FileChangedReason.Changed, FileChangedReason.Changed),
            new EventTransition(FileChangedReason.None, FileChangedReason.Removed, FileChangedReason.Removed),

            new EventTransition(FileChangedReason.Added, FileChangedReason.None, FileChangedReason.Added),
            new EventTransition(FileChangedReason.Added, FileChangedReason.Added, FileChangedReason.Added),
            new EventTransition(FileChangedReason.Added, FileChangedReason.Changed, FileChangedReason.Added),
            new EventTransition(FileChangedReason.Added, FileChangedReason.Removed, FileChangedReason.None),

            new EventTransition(FileChangedReason.Changed, FileChangedReason.None, FileChangedReason.Changed),
            new EventTransition(FileChangedReason.Changed, FileChangedReason.Added, FileChangedReason.Changed),
            new EventTransition(FileChangedReason.Changed, FileChangedReason.Changed, FileChangedReason.Changed),
            new EventTransition(FileChangedReason.Changed, FileChangedReason.Removed, FileChangedReason.Removed),

            new EventTransition(FileChangedReason.Removed, FileChangedReason.None, FileChangedReason.Removed),
            new EventTransition(FileChangedReason.Removed, FileChangedReason.Added, FileChangedReason.Changed),
            new EventTransition(FileChangedReason.Removed, FileChangedReason.Changed, FileChangedReason.Changed),
            new EventTransition(FileChangedReason.Removed, FileChangedReason.Removed, FileChangedReason.Removed)
        };

        private List<FileChangedEventArgs> _events = new List<FileChangedEventArgs>();
        private object _eventsLock = new object();
        private void AddEvent(string file, FileChangedReason reason)
        {
            lock (_eventsLock)
            {
                _events.Add(new FileChangedEventArgs(file, reason));
            }
            Task.Delay(500).ContinueWith(task =>
            {
                lock (_eventsLock)
                {
                    if (_events.Any())
                    {
                        try
                        {
                            if (Changed != null)
                            {
                                var events = new Dictionary<string, FileChangedReason>();
                                foreach (var evt in _events)
                                {
                                    FileChangedReason prevReason;
                                    if (!events.TryGetValue(evt.File, out prevReason))
                                    {
                                        prevReason = FileChangedReason.None;
                                    }
                                    var newReason = _eventTransitions
                                        .Single(t => t.PreviousReason == prevReason && t.CurrentReason == evt.ChangedReason)
                                        .NewReason;
                                    events[evt.File] = newReason;
                                }
                                foreach (var evt in events.Where(x => x.Value != FileChangedReason.None))
                                {
                                    Changed(this, new FileChangedEventArgs(evt.Key, evt.Value));
                                }
                            }
                        }
                        finally
                        {
                            _events.Clear();
                        }
                    }
                }
            });
        }

        public event EventHandler<FileChangedEventArgs> Changed;

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            AddEvent(e.FullPath, FileChangedReason.Added);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            AddEvent(e.FullPath, FileChangedReason.Changed);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            AddEvent(e.FullPath, FileChangedReason.Removed);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            AddEvent(e.OldFullPath, FileChangedReason.Removed);
            AddEvent(e.FullPath, FileChangedReason.Added);
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
                if (_watcher != null)
                {
                    _watcher.Dispose();
                    _watcher = null;
                }
            }
        }

        ~FileWatcher()
        {
            Dispose(false);
        }
    }
}