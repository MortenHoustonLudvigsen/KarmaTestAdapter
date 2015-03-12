using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaSettings
    {
        public KarmaSettings(string configFile, IKarmaLogger logger)
        {
            AreValid = false;
            HasSettingsFile = false;
            Logger = logger;

            try
            {
                Directory = Path.GetDirectoryName(configFile);
                KarmaConfigFile = GetFullPath(Globals.KarmaConfigFilename);
                SettingsFile = GetFullPath(Globals.SettingsFilename);

                if (PathUtils.PathHasFileName(configFile, Globals.SettingsFilename) && File.Exists(configFile))
                {
                    Json.PopulateFromFile(configFile, this);
                    AreValid = true;
                    HasSettingsFile = true;
                }
                else if (PathUtils.PathHasFileName(configFile, Globals.KarmaConfigFilename) && File.Exists(configFile))
                {
                    AreValid = true;
                }

                if (AreValid)
                {
                    LogDirectory = GetFullPath(LogDirectory ?? "");
                    if (LogToFile)
                    {
                        logger.AddLogger(LogFilePath);
                    }
                }
                else
                {
                    LogDirectory = "";
                    LogToFile = false;
                }
            }
            catch (Exception ex)
            {
                AreValid = false;
                logger.Error(ex, "Could not read settings");
            }
        }

        /// <summary>
        /// The Karma configuration file
        /// </summary>
        public string KarmaConfigFile { get; set; }

        /// <summary>
        /// True if the Karma test adapter should be disabled for this karma configuration file
        /// </summary>
        public bool Disabled { get; set; }

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
        /// Indicates whether settings have been loaded successfully
        /// </summary>
        [JsonIgnore]
        public bool AreValid { get; private set; }

        /// <summary>
        /// The path of the adapter settings file
        /// </summary>
        [JsonIgnore]
        public string SettingsFile { get; private set; }

        /// <summary>
        /// True if a settings file (KarmaTestAdapter.json) is used
        /// </summary>
        [JsonIgnore]
        public bool HasSettingsFile { get; private set; }

        /// <summary>
        /// Directory of the settings file
        /// </summary>
        [JsonIgnore]
        public string Directory { get; private set; }

        /// <summary>
        /// The logger
        /// </summary>
        [JsonIgnore]
        public IKarmaLogger Logger { get; private set; }

        /// <summary>
        /// The file to log to when LogToFile == true
        /// </summary>
        public string LogFilePath { get { return GetFullPath(LogDirectory, Globals.LogFilename); } }


        private string GetFullPath(params string[] paths)
        {
            return GetFullPath(Path.Combine(paths));
        }

        private string GetFullPath(string path)
        {
            return PathUtils.GetFullPath(string.IsNullOrWhiteSpace(path) ? "." : path, Directory);
        }
    }
}