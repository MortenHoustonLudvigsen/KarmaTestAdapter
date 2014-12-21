using KarmaTestAdapter.KarmaTestResults;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public abstract class KarmaTestContainerBase : IKarmaTestContainer, IDisposable
    {
        public KarmaTestContainerBase(KarmaTestContainerDiscoverer discoverer, string source)
            : this(discoverer, source, DateTime.Now)
        {
        }

        public KarmaTestContainerBase(KarmaTestContainerDiscoverer discoverer, string source, DateTime timeStamp)
        {
            KarmaTestContainerDiscoverer = discoverer;
            Source = source;
            TimeStamp = timeStamp;
        }

        public DateTime TimeStamp { get; protected set; }
        public KarmaTestContainerDiscoverer KarmaTestContainerDiscoverer { get; private set; }
        public string Source { get; private set; }
        public ITestContainerDiscoverer Discoverer { get { return KarmaTestContainerDiscoverer; } }
        public IEnumerable<Guid> DebugEngines { get { return Enumerable.Empty<Guid>(); } }
        public bool IsAppContainerTestContainer { get { return false; } }
        public FrameworkVersion TargetFramework { get { return FrameworkVersion.None; } }
        public Architecture TargetPlatform { get { return Architecture.AnyCPU; } }

        public int CompareTo(ITestContainer other)
        {
            var testContainer = other as IKarmaTestContainer;
            if (testContainer == null)
            {
                return -1;
            }

            var result = String.Compare(this.Source, testContainer.Source, StringComparison.OrdinalIgnoreCase);
            if (result != 0)
            {
                return result;
            }

            return this.TimeStamp.CompareTo(testContainer.TimeStamp);
        }

        public IDeploymentData DeployAppContainer()
        {
            return null;
        }

        public ITestContainer Snapshot()
        {
            return new KarmaTestContainerSnapshot(this);
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
            }
        }

        ~KarmaTestContainerBase()
        {
            Dispose(false);
        }
    }
}
