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
    public class KarmaSettings: IDisposable
    {
        public static KarmaSettings Read(string path, IKarmaLogger logger)
        {
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
                    logger.Error(ex);
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
            if (string.IsNullOrWhiteSpace(settings.LogDirectory))
            {
                settings.LogDirectory = settings.Directory;
            }
            if (string.IsNullOrWhiteSpace(settings.OutputDirectory))
            {
                settings.OutputDirectory = null;
            }
            settings.Logger = logger;
            if (settings.LogToFile)
            {
                logger.AddLogger(settings.LogFilePath);
            }
            else
            {
                logger.RemoveLogger(settings.LogFilePath);
            }
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
        public string LogDirectory { get; set; }
        public string OutputDirectory { get; set; }

        public IKarmaLogger Logger { get; private set; }

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
        public string GetOutputFile(string outputDirectory)
        {
            return GetFilename(outputDirectory, Globals.OutputFilename);
        }

        /// <summary>
        /// The file for VsConfig
        /// </summary>
        public string GetVsConfigFilename(string outputDirectory)
        {
            return GetFilename(outputDirectory, Globals.VsConfigFilename);
        }

        private string GetFilename(string outputDirectory, string filename)
        {
            if (!string.IsNullOrWhiteSpace(OutputDirectory))
            {
                if (!System.IO.Directory.Exists(outputDirectory))
                {
                    System.IO.Directory.CreateDirectory(outputDirectory);
                }
                return Path.Combine(outputDirectory, filename);
            }
            else
            {
                return Path.GetTempFileName();
            }
        }

        private string GetFullPath(params string[] paths)
        {
            var oldCwd = System.IO.Directory.GetCurrentDirectory();
            try
            {
                System.IO.Directory.SetCurrentDirectory(Directory);
                return Path.GetFullPath(Path.Combine(paths));
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
