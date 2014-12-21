using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public class KarmaTestContainerList : IEnumerable<KarmaTestContainer>, IDisposable
    {
        public KarmaTestContainerList(KarmaTestContainerDiscoverer discoverer)
        {
            Discoverer = discoverer;
        }

        private List<KarmaTestContainer> _containers = new List<KarmaTestContainer>();
        public KarmaTestContainerDiscoverer Discoverer { get; private set; }

        public void CreateContainer(string source)
        {
            var container = new KarmaTestContainer(this, source, Discoverer.Logger);
            _containers.Add(container);
        }

        public void Remove(KarmaTestContainer container)
        {
            container.Dispose();
            _containers.Remove(container);
        }

        public void Clear()
        {
            _containers.ForEach(c => c.Dispose());
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
