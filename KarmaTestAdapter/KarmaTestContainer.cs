using KarmaTestAdapter.Commands;
using KarmaTestAdapter.Config;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using Newtonsoft.Json;
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
        private Dictionary<string, string> _files = new Dictionary<string, string>();

        public KarmaTestContainer(ITestContainerDiscoverer discoverer, string source, IKarmaLogger logger)
            : this(discoverer, source, logger, Enumerable.Empty<Guid>(), null, null)
        { }

        private KarmaTestContainer(ITestContainerDiscoverer discoverer, string source, IKarmaLogger logger, IEnumerable<Guid> debugEngines, KarmaConfig config, Dictionary<string, string> files)
        {
            this.Source = source;
            this.Logger = logger;
            this.Settings = KarmaSettings.Read(source, logger);
            this.DebugEngines = debugEngines;
            this._discoverer = discoverer;
            this.TargetFramework = FrameworkVersion.None;
            this.TargetPlatform = Architecture.AnyCPU;
            this._timeStamp = DateTime.Now;
            this.Config = config ?? KarmaGetConfigCommand.GetConfig(Source, Logger);
            this._files = files;
            if (this._files == null || this._files.Count == 0)
            {
                this._files = GetFiles();
            }
        }

        private KarmaTestContainer(KarmaTestContainer copy, DateTime timeStamp)
            : this(copy._discoverer, copy.Source, copy.Logger, copy.DebugEngines, copy.Config, copy._files)
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
        public KarmaConfig Config { get; private set; }
        public bool ShouldRefresh { get; private set; }

        private Dictionary<string, string> GetFiles()
        {
            return Config.GetFiles().ToDictionary(f => f, f => Sha1Utils.GetHash(f), StringComparer.InvariantCultureIgnoreCase);
        }

        private string GetCurrentHash(string file)
        {
            string hash;
            if (_files.TryGetValue(file, out hash))
            {
                return hash;
            }
            return null;
        }

        public bool FileAdded(string file)
        {
            return FileChanged(file);
        }

        public bool FileChanged(string file)
        {
            if (_files.ContainsKey(file) || Config.HasFile(file))
            {
                // The file belongs to this container
                var newHash = Sha1Utils.GetHash(file);
                if (newHash != GetCurrentHash(file))
                {
                    _files[file] = newHash;
                    ShouldRefresh = true;
                    return true;
                }
            }
            return false;
        }

        public bool FileRemoved(string file)
        {
            if (_files.ContainsKey(file) || Config.HasFile(file))
            {
                // The file belongs to this container
                _files.Remove(file);
                ShouldRefresh = true;
                return true;
            }
            return false;
        }

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

        public KarmaTestContainer Refresh()
        {
            return ShouldRefresh ? new KarmaTestContainer(this, DateTime.Now) : this;
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
