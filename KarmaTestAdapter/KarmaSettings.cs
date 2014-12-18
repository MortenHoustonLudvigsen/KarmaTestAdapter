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
    public class KarmaSettings
    {
        public static KarmaSettings Read(string path, IKarmaLogger logger)
        {
            logger.Info("Reading settings from {0}", path);
            KarmaSettings settings = null;

            if (PathUtils.PathHasFileName(path, Globals.SettingsFilename) && File.Exists(path))
            {
                try
                {
                    settings = JsonConvert.DeserializeObject<KarmaSettings>(File.ReadAllText(path, Encoding.UTF8));
                    settings.SettingsFile = path;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }
            else
            {
                settings = new KarmaSettings
                {
                    KarmaConfigFile = Path.GetFileName(path)
                };
            }

            settings.Directory = Path.GetDirectoryName(path);

            logger.Info("settings.KarmaConfigFile: {0}", settings.KarmaConfigFile);
            logger.Info("settings.SettingsFile:    {0}", settings.SettingsFile);
            logger.Info("settings.LogToFile:       {0}", settings.LogToFile);
            logger.Info("settings.Directory:       {0}", settings.Directory);

            return settings;
        }

        private KarmaSettings()
        {
            KarmaConfigFile = null;
        }

        private string _karmaConfigFile;
        public string KarmaConfigFile
        {
            get { return _karmaConfigFile; }
            set { _karmaConfigFile = string.IsNullOrWhiteSpace(value) ? Globals.KarmaSettingsFilename : value; }
        }

        public string SettingsFile { get; set; }

        /// <summary>
        /// Should logging be done to a file as well as normal logging
        /// </summary>
        public bool LogToFile { get; set; }

        public string Directory { get; private set; }

        /// <summary>
        /// The file to log to when LogToFile == true
        /// </summary>
        public string LogFilePath { get { return Path.Combine(Directory, Globals.LogFilename); } }

        /// <summary>
        /// The file for karma output
        /// </summary>
        public string GetOutputFile()
        {
            return LogToFile ? Path.Combine(Directory, Globals.OutputFilename) : Path.GetTempFileName();
        }

        /// <summary>
        /// The file for VsConfig
        /// </summary>
        public string GetVsConfigFilename()
        {
            return LogToFile ? Path.Combine(Directory, Globals.VsConfigFilename) : Path.GetTempFileName();
        }
    }
}
