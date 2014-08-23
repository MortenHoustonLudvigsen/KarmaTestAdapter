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
            if (PathUtils.PathHasFileName(path, Globals.SettingsFilename))
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    try
                    {
                        var settings = JsonConvert.DeserializeObject<KarmaSettings>(File.ReadAllText(path, Encoding.UTF8));
                        settings.SettingsFile = path;
                        return settings;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);

                    }
                }
            }
            return new KarmaSettings
            {
                KarmaConfigFile = path
            };
        }

        private KarmaSettings()
        {
        }

        private string _karmaConfigFile;
        public string KarmaConfigFile
        {
            get { return _karmaConfigFile; }
            private set { _karmaConfigFile = string.IsNullOrWhiteSpace(value) ? Globals.KarmaSettingsFilename : value; }
        }

        public string SettingsFile { get; private set; }
    }
}
