using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public class KarmaSettings : IDisposable
    {
        public KarmaSettings(string source, IKarmaLogger logger)
        {
            Source = source;
            Logger = logger;
            Directory = Path.GetDirectoryName(source);
            if (PathUtils.PathHasFileName(source, Globals.SettingsFilename) && File.Exists(source))
            {
                JsonConvert.PopulateObject(File.ReadAllText(source, Encoding.UTF8), this);
                SettingsFile = source;
                KarmaConfigFile = GetFullPath(KarmaConfigFile ?? Globals.KarmaConfigFilename);
            }
            else
            {
                SettingsFile = GetFullPath(Globals.SettingsFilename);
                KarmaConfigFile = source;
            }
            LogDirectory = GetFullPath(LogDirectory ?? "");
            OutputDirectory = !string.IsNullOrWhiteSpace(OutputDirectory) ? OutputDirectory : null;
            if (LogToFile)
            {
                logger.AddLogger(LogFilePath);
            }
        }

        /// <summary>
        /// The source file of the settings - can be either an adapter settings file (named <c>karma-vs-reporter.json</c>) or a
        /// Karma configuration file (default name <c>karma.conf.js</c>)
        /// </summary>
        [JsonIgnore]
        public string Source { get; private set; }

        [JsonIgnore]
        public IKarmaLogger Logger { get; private set; }

        /// <summary>
        /// The path of the adapter settings file
        /// </summary>
        [JsonIgnore]
        public string SettingsFile { get; private set; }

        /// <summary>
        /// Directory of the settings file
        /// </summary>
        [JsonIgnore]
        public string Directory { get; private set; }

        /// <summary>
        /// The Karma configuration file
        /// </summary>
        public string KarmaConfigFile { get; set; }

        /// <summary>
        /// Should Karma be started in the background
        /// </summary>
        public bool ServerMode { get; set; }

        /// <summary>
        /// Port number to use if Karma should be started in the background
        /// </summary>
        public int? ServerPort { get; set; }

        public bool ServerModeValid { get { return ServerMode && ServerPort.HasValue; } }

        /// <summary>
        /// Should logging be done to a file as well as normal logging
        /// </summary>
        public bool LogToFile { get; set; }

        /// <summary>
        /// Where the log file should be saved (if LogToFile is true). If this property is not specified the directory in which
        /// karma-vs-reporter.json resides is used.
        /// </summary>
        public string LogDirectory { get; set; }

        /// <summary>
        /// Normally the adapter communicates with Karma using temporary files. These files are deleted immediately. If you want
        /// to see these files, you can specify an OutputDirectory, in which case the files will not be deleted.
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// The file to log to when LogToFile == true
        /// </summary>
        public string LogFilePath { get { return GetFullPath(LogDirectory, Globals.LogFilename); } }

        public string GetOutputDirectory(string command)
        {
            if (!string.IsNullOrWhiteSpace(OutputDirectory))
            {

                var timestamp = DateTime.Now.ToString("yyyyMMddTHHmmss");
                var outputDirectory = GetFullPath(OutputDirectory, string.Format("{0}.{1}", command, timestamp));
                var _outputNo = 1;
                while (System.IO.Directory.Exists(outputDirectory))
                {
                    _outputNo += 1;
                    outputDirectory = GetFullPath(OutputDirectory, string.Format("{0}.{1}-{2}", command, timestamp, _outputNo));
                }
                return outputDirectory;
            }
            return null;
        }

        /// <summary>
        /// The file for karma output
        /// </summary>
        public KarmaOutputFile GetOutputFile(string outputDirectory)
        {
            return new KarmaOutputFile(outputDirectory, Globals.OutputFilename);
        }

        /// <summary>
        /// The file for VsConfig
        /// </summary>
        public KarmaOutputFile GetVsConfigFilename(string outputDirectory)
        {
            return new KarmaOutputFile(outputDirectory, Globals.VsConfigFilename);
        }

        private string GetFullPath(params string[] paths)
        {
            return GetFullPath(Path.Combine(paths));
        }

        private string GetFullPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = ".";
            }
            var oldCwd = System.IO.Directory.GetCurrentDirectory();
            try
            {
                System.IO.Directory.SetCurrentDirectory(Directory);
                return Path.GetFullPath(path);
            }
            finally
            {
                System.IO.Directory.SetCurrentDirectory(oldCwd);
            }
        }

        public void Dispose()
        {
            if (Logger != null)
            {
                Logger.RemoveLogger(LogFilePath);
            }
        }
    }
}
