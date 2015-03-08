using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestContainerBase<TDiscoverer> : ITestContainer, IDisposable
        where TDiscoverer : ITestContainerDiscoverer
    {
        public KarmaTestContainerBase(TDiscoverer discoverer, string source)
            : this(discoverer, source, DateTime.Now)
        {
        }

        public KarmaTestContainerBase(TDiscoverer discoverer, string source, DateTime timeStamp)
        {
            Discoverer = discoverer;
            Source = source;
            TimeStamp = timeStamp;
        }

        public DateTime TimeStamp { get; protected set; }
        public TDiscoverer Discoverer { get; private set; }

        #region ITestContainer

        public string Source { get; private set; }
        ITestContainerDiscoverer ITestContainer.Discoverer { get { return Discoverer; } }
        public IEnumerable<Guid> DebugEngines { get { return Enumerable.Empty<Guid>(); } }
        public bool IsAppContainerTestContainer { get { return false; } }
        public FrameworkVersion TargetFramework { get { return FrameworkVersion.None; } }
        public Architecture TargetPlatform { get { return Architecture.AnyCPU; } }

        public int CompareTo(ITestContainer other)
        {
            var testContainer = other as KarmaTestContainerBase<TDiscoverer>;
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
            return new KarmaTestContainerSnapshot<TDiscoverer>(this);
        }

        #endregion

        #region IDisposable

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
            if (_disposed) return;

            if (disposing)
            {
            }

            _disposed = true;
        }

        ~KarmaTestContainerBase()
        {
            Dispose(false);
        }

        #endregion
    }
}