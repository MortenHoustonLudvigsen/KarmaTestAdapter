using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.Shell.Interop;
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
        public KarmaSettings(string configFile, Func<string, bool> fileExists, string baseDirectory, ITestLogger logger)
        {
            HasSettingsFile = false;
            Logger = logger;

            try
            {
                Directory = Path.GetDirectoryName(configFile);
                SettingsFile = GetFullPath(Globals.SettingsFilename);
                KarmaConfigFile = GetFullPath(Globals.KarmaConfigFilename);

                if (PathUtils.PathHasFileName(configFile, Globals.SettingsFilename))
                {
                    Json.PopulateFromFile(configFile, this);
                    KarmaConfigFile = string.IsNullOrWhiteSpace(KarmaConfigFile) ? GetFullPath(Globals.KarmaConfigFilename) : GetFullPath(KarmaConfigFile);
                    _validator.Validate(fileExists(SettingsFile), "Settings file not found: {0}", PathUtils.GetRelativePath(baseDirectory, SettingsFile));
                    _validator.Validate(fileExists(KarmaConfigFile), "Karma configuration file not found: {0}", PathUtils.GetRelativePath(baseDirectory, KarmaConfigFile));
                    HasSettingsFile = true;
                }
                else
                {
                    KarmaConfigFile = GetFullPath(configFile);
                    _validator.Validate(fileExists(KarmaConfigFile), "Karma configuration file not found: {0}", PathUtils.GetRelativePath(baseDirectory, KarmaConfigFile));
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
                _validator.Validate(false, "Could not read settings: {0}", ex.Message);
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

        private readonly Validator _validator = new Validator();

        /// <summary>
        /// Indicates whether settings have been loaded successfully
        /// </summary>
        [JsonIgnore]
        public bool AreValid { get { return _validator.IsValid; } }

        /// <summary>
        /// Indicates the reason why the settings are invalid
        /// </summary>
        /// [JsonIgnore]
        public string InvalidReason { get { return _validator.InvalidReason; } }

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
        public ITestLogger Logger { get; private set; }

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