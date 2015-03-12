using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestContainerSourceInfo
    {
        public KarmaTestContainerSourceInfo(IVsProject project, string source)
        {
            Project = project;
            Source = PathUtils.GetPhysicalPath(source);
        }

        public IVsProject Project { get; private set; }
        public string Source { get; private set; }
    }

    public class KarmaTestContainerList : IEnumerable<KarmaTestContainer>, IDisposable
    {
        public KarmaTestContainerList(KarmaTestContainerDiscoverer discoverer)
        {
            Discoverer = discoverer;
        }

        private List<KarmaTestContainer> _containers = new List<KarmaTestContainer>();
        public KarmaTestContainerDiscoverer Discoverer { get; private set; }

        public void CreateContainer(KarmaTestContainerSourceInfo source)
        {
            try
            {
                RemoveFromDirectory(Path.GetDirectoryName(source.Source));
                _containers.Add(new KarmaTestContainer(this, source.Project, source.Source));
            }
            catch (Exception ex)
            {
                Discoverer.Logger.Error(ex, "Failed to create test container for {0}", source);
            }
        }

        public void CreateContainers(IEnumerable<KarmaTestContainerSourceInfo> sources)
        {
            foreach (var source in sources)
            {
                CreateContainer(source);
            }
        }

        public void Remove(IVsProject project)
        {
            var containersToRemove = this.Where(c => c.Project == project).ToList();
            foreach (var container in containersToRemove)
            {
                Remove(container);
            }
        }

        public void Remove(IEnumerable<string> source)
        {
            var containersToRemove = this.Where(c => source.Any(s => PathUtils.PathsEqual(c.Source, s))).ToList();
            foreach (var container in containersToRemove)
            {
                Remove(container);
            }
        }

        public void Remove(string source)
        {
            var containersToRemove = this.Where(c => PathUtils.PathsEqual(c.Source, source)).ToList();
            foreach (var container in containersToRemove)
            {
                Remove(container);
            }
        }

        public void RemoveFromDirectory(string directory)
        {
            var containersToRemove = this.Where(c => PathUtils.IsInDirectory(c.Source, directory)).ToList();
            foreach (var container in containersToRemove)
            {
                Remove(container);
            }
        }

        public void Remove(KarmaTestContainer container)
        {
            _containers.Remove(container);
            container.Dispose();
        }

        public void Clear()
        {
            _containers.ToList().ForEach(c => c.Dispose());
            _containers.Clear();
        }

        public IEnumerator<KarmaTestContainer> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                    Clear();
                    _containers = null;
                }
            }
        }

        ~KarmaTestContainerList()
        {
            Dispose(false);
        }
    }
}