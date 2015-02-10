using KarmaTestAdapter.Config;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
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
            AreValid = false;
            Source = source;
            Logger = logger;

            try
            {
                Directory = Path.GetDirectoryName(source);

                if (PathUtils.PathHasFileName(source, Globals.SettingsFilename) && File.Exists(source))
                {
                    try
                    {
                        Json.PopulateFromFile(source, this);
                        SettingsFile = source;
                        KarmaConfigFile = GetFullPath(KarmaConfigFile ?? Globals.KarmaConfigFilename);
                        AreValid = true;
                    }
                    catch (Exception ex)
                    {
                        SettingsFile = GetFullPath(source);
                        KarmaConfigFile = GetFullPath(Globals.KarmaConfigFilename);
                        throw ex;
                    }
                }
                else
                {
                    SettingsFile = GetFullPath(Globals.SettingsFilename);
                    KarmaConfigFile = source;
                    AreValid = true;
                }
                if (AreValid)
                {
                    LogDirectory = GetFullPath(LogDirectory ?? "");
                    OutputDirectory = !string.IsNullOrWhiteSpace(OutputDirectory) ? OutputDirectory : null;
                    if (LogToFile)
                    {
                        logger.AddLogger(LogFilePath);
                    }
                    TestFilesSpec = this.GetTestFilesSpec();
                }
                else
                {
                    LogDirectory = "";
                    OutputDirectory = null;
                    LogToFile = false;
                    TestFilesSpec = this.GetTestFilesSpec();
                }
            }
            catch (Exception ex)
            {
                AreValid = false;
                SettingsFile = SettingsFile ?? GetFullPath(Globals.SettingsFilename);
                KarmaConfigFile = KarmaConfigFile ?? GetFullPath(Globals.KarmaConfigFilename);
                TestFilesSpec = null;
                logger.Error(ex, "Could not read settings");
            }
        }

        /// <summary>
        /// Indicates whether settings have been loaded successfully
        /// </summary>
        [JsonIgnore]
        public bool AreValid { get; private set; }

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
        /// List of files patterns for files containing tests.
        /// </summary>
        public List<string> TestFiles { get; set; }

        [JsonIgnore]
        public FilesSpec TestFilesSpec { get; private set; }

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
            return PathUtils.GetFullPath(string.IsNullOrWhiteSpace(path) ? "." : path, Directory);
        }

        private FilesSpec GetTestFilesSpec()
        {
            if (TestFiles == null)
            {
                return null;
            }
            try
            {
                var testFilesSpec = new FilesSpec();
                testFilesSpec.Include(Directory, TestFiles);
                return testFilesSpec;
            }
            catch (Exception ex)
            {
                throw ex.Wrap("Could not read TestFiles");
            }
        }

        public void Dispose()
        {
            if (Logger != null)
            {
                if (AreValid)
                {
                    Logger.RemoveLogger(LogFilePath);
                }
            }
        }
    }
}
