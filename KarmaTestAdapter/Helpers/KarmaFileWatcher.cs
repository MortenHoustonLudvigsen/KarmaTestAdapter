using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Helpers
{
    public class KarmaFileWatcher : IDisposable
    {
        public KarmaFileWatcher(string directory, string filter, bool includeSubdirectories = true)
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
            public EventTransition(TestFileChangedReason previousReason, TestFileChangedReason currentReason, TestFileChangedReason newReason)
            {
                PreviousReason = previousReason;
                CurrentReason = currentReason;
                NewReason = newReason;
            }

            public TestFileChangedReason PreviousReason { get; private set; }
            public TestFileChangedReason CurrentReason { get; private set; }
            public TestFileChangedReason NewReason { get; private set; }
        }

        private static readonly List<EventTransition> _eventTransitions = new List<EventTransition>
        {
            new EventTransition(TestFileChangedReason.None, TestFileChangedReason.None, TestFileChangedReason.None),
            new EventTransition(TestFileChangedReason.None, TestFileChangedReason.Added, TestFileChangedReason.Added),
            new EventTransition(TestFileChangedReason.None, TestFileChangedReason.Changed, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.None, TestFileChangedReason.Saved, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.None, TestFileChangedReason.Removed, TestFileChangedReason.Removed),

            new EventTransition(TestFileChangedReason.Added, TestFileChangedReason.None, TestFileChangedReason.Added),
            new EventTransition(TestFileChangedReason.Added, TestFileChangedReason.Added, TestFileChangedReason.Added),
            new EventTransition(TestFileChangedReason.Added, TestFileChangedReason.Changed, TestFileChangedReason.Added),
            new EventTransition(TestFileChangedReason.Added, TestFileChangedReason.Saved, TestFileChangedReason.Added),
            new EventTransition(TestFileChangedReason.Added, TestFileChangedReason.Removed, TestFileChangedReason.None),

            new EventTransition(TestFileChangedReason.Changed, TestFileChangedReason.None, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Changed, TestFileChangedReason.Added, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Changed, TestFileChangedReason.Changed, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Changed, TestFileChangedReason.Saved, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Changed, TestFileChangedReason.Removed, TestFileChangedReason.Removed),

            new EventTransition(TestFileChangedReason.Saved, TestFileChangedReason.None, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Saved, TestFileChangedReason.Added, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Saved, TestFileChangedReason.Changed, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Saved, TestFileChangedReason.Saved, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Saved, TestFileChangedReason.Removed, TestFileChangedReason.Removed),

            new EventTransition(TestFileChangedReason.Removed, TestFileChangedReason.None, TestFileChangedReason.Removed),
            new EventTransition(TestFileChangedReason.Removed, TestFileChangedReason.Added, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Removed, TestFileChangedReason.Changed, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Removed, TestFileChangedReason.Saved, TestFileChangedReason.Changed),
            new EventTransition(TestFileChangedReason.Removed, TestFileChangedReason.Removed, TestFileChangedReason.Removed)
        };

        private List<TestFileChangedEventArgs> _events = new List<TestFileChangedEventArgs>();
        private object _eventsLock = new object();
        private void AddEvent(string file, TestFileChangedReason reason)
        {
            lock (_eventsLock)
            {
                _events.Add(new TestFileChangedEventArgs(file, reason));
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
                                var events = new Dictionary<string, TestFileChangedReason>();
                                foreach (var evt in _events)
                                {
                                    TestFileChangedReason prevReason;
                                    if (!events.TryGetValue(evt.File, out prevReason))
                                    {
                                        prevReason = TestFileChangedReason.None;
                                    }
                                    var newReason = _eventTransitions
                                        .Single(t => t.PreviousReason == prevReason && t.CurrentReason == evt.ChangedReason)
                                        .NewReason;
                                    events[evt.File] = newReason;
                                }
                                foreach (var evt in events.Where(x => x.Value != TestFileChangedReason.None))
                                {
                                    Changed(this, new TestFileChangedEventArgs(evt.Key, evt.Value));
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

        public event EventHandler<TestFileChangedEventArgs> Changed;

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            AddEvent(e.FullPath, TestFileChangedReason.Added);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            AddEvent(e.FullPath, TestFileChangedReason.Changed);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            AddEvent(e.FullPath, TestFileChangedReason.Removed);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            AddEvent(e.OldFullPath, TestFileChangedReason.Removed);
            AddEvent(e.FullPath, TestFileChangedReason.Added);
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

        ~KarmaFileWatcher()
        {
            Dispose(false);
        }
    }
}
