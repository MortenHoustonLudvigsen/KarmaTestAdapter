using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JsTestAdapter.TestAdapter
{
    public class TestContainerList : IEnumerable<TestContainer>, IDisposable
    {
        public TestContainerList(TestContainerDiscoverer discoverer)
        {
            Discoverer = discoverer;
        }

        private List<TestContainer> _containers = new List<TestContainer>();
        public TestContainerDiscoverer Discoverer { get; private set; }

        public void CreateContainer(TestContainerSource source)
        {
            try
            {
                RemoveFromDirectory(source.SourceDirectory);
                if (!string.IsNullOrWhiteSpace(source.Source))
                {
                    _containers.Add(Discoverer.CreateTestContainer(source));
                }
                RemoveDuplicates();
                Discoverer.RefreshTestContainers();
            }
            catch (Exception ex)
            {
                Discoverer.Logger.Error(ex, "Failed to create test container for {0}", source.Source);
            }
        }

        public void CreateContainers(IEnumerable<TestContainerSource> sources)
        {
            foreach (var source in sources)
            {
                CreateContainer(source);
            }
        }

        public void RemoveDuplicates()
        {
            var containersToRemove = this.Where(c => this.Any(d => d.IsDuplicate(c))).ToList();

            foreach (var container in containersToRemove)
            {
                Remove(container);
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

        public void Remove(TestContainer container)
        {
            _containers.Remove(container);
            container.Dispose();
            Discoverer.RefreshTestContainers();
        }

        public void Clear()
        {
            _containers.ToList().ForEach(c => c.Dispose());
            _containers.Clear();
            Discoverer.RefreshTestContainers();
        }

        public IEnumerator<TestContainer> GetEnumerator()
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

        ~TestContainerList()
        {
            Dispose(false);
        }
    }
}