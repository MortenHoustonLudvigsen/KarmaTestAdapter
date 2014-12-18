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

namespace KarmaTestAdapter
{
    public class KarmaTestContainer : IKarmaTestContainer
    {
        private readonly DateTime _timeStamp;
        private ITestContainerDiscoverer _discoverer;

        public KarmaTestContainer(ITestContainerDiscoverer discoverer, string source, IKarmaLogger logger)
            : this(discoverer, source, logger, Enumerable.Empty<Guid>())
        { }

        public KarmaTestContainer(ITestContainerDiscoverer discoverer, string source, IKarmaLogger logger, IEnumerable<Guid> debugEngines)
        {
            this.Source = source;
            this.Logger = logger;
            this.Settings = KarmaSettings.Read(source, logger);
            this.DebugEngines = debugEngines;
            this._discoverer = discoverer;
            this.TargetFramework = FrameworkVersion.None;
            this.TargetPlatform = Architecture.AnyCPU;
            this._timeStamp = DateTime.Now;
        }

        private KarmaTestContainer(KarmaTestContainer copy, DateTime timeStamp)
            : this(copy._discoverer, copy.Source, copy.Logger, copy.DebugEngines)
        {
            this._timeStamp = timeStamp;
        }

        public string Source { get; set; }
        public IKarmaLogger Logger { get; private set; }
        public KarmaSettings Settings { get; private set; }
        public Uri ExecutorUri { get { return Globals.ExecutorUri; } }
        public Karma Karma { get; set; }
        public IEnumerable<Guid> DebugEngines { get; set; }
        public FrameworkVersion TargetFramework { get; set; }
        public Architecture TargetPlatform { get; set; }

        public int CompareTo(ITestContainer other)
        {
            var testContainer = other as KarmaTestContainer;
            if (testContainer == null)
            {
                return -1;
            }

            var result = String.Compare(this.Source, testContainer.Source, StringComparison.OrdinalIgnoreCase);
            if (result != 0)
            {
                return result;
            }

            return this._timeStamp.CompareTo(testContainer._timeStamp);
        }

        public IDeploymentData DeployAppContainer()
        {
            return null;
        }

        public ITestContainerDiscoverer Discoverer
        {
            get { return _discoverer; }
        }

        public bool IsAppContainerTestContainer
        {
            get { return false; }
        }

        public ITestContainer Snapshot()
        {
            return new KarmaTestContainer(this, _timeStamp);
        }

        public KarmaTestContainer FreshCopy()
        {
            return new KarmaTestContainer(this, DateTime.Now);
        }

        public override string ToString()
        {
            return this.ExecutorUri.ToString() + "/" + this.Source;
        }
    }
}
