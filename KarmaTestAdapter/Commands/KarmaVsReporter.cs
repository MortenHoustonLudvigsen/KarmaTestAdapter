using KarmaTestAdapter.Helpers;
using Newtonsoft.Json.Linq;
using Summerset.SemanticVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO = System.IO;

namespace KarmaTestAdapter.Commands
{
    public class KarmaVsReporter
    {
        public KarmaVsReporter(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException("directory");
            }

            while (!string.IsNullOrWhiteSpace(directory) && !SetDirectory(directory))
            {
                directory = IO.Path.GetDirectoryName(directory);
            }
        }

        public string Reporter { get; private set; }
        public string ConfigFile { get; private set; }
        public string Directory { get; private set; }
        public DateTime? Timestamp { get; private set; }

        private bool SetDirectory(string directory)
        {
            var nodeModules = IO.Path.Combine(directory, "node_modules");
            Directory = directory;
            Reporter = IO.Path.Combine(nodeModules, "karma-vs-reporter", "karma-vs-reporter");
            ConfigFile = IO.Path.Combine(nodeModules, "karma-vs-reporter", "package.json");
            Timestamp = Exists ? IO.File.GetLastWriteTime(ConfigFile) : (DateTime?)null;
            return IO.Directory.Exists(nodeModules);
        }

        private ISemanticVersion _version = null;
        public ISemanticVersion Version
        {
            get
            {
                if (Exists)
                {
                    var newTimestamp = GetTimestamp();
                    if (_version == null || newTimestamp != Timestamp)
                    {
                        Timestamp = newTimestamp;
                        var config = Json.ParseFile(ConfigFile);
                        JToken version;
                        if (config.TryGetValue("version", out version) && version.Type == JTokenType.String)
                        {
                            _version = new SemanticVersion(version.ToString());
                        }
                        else
                        {
                            _version = null;
                        }
                    }
                }
                else
                {
                    _version = null;
                }
                return _version;
            }
        }

        public bool Exists { get { return IO.File.Exists(Reporter) && IO.File.Exists(ConfigFile); } }

        private DateTime? GetTimestamp()
        {
            return Exists ? IO.File.GetLastWriteTime(ConfigFile) : (DateTime?)null;
        }
    }
}
