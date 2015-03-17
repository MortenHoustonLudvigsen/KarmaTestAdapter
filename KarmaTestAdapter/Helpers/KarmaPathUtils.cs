using JsTestAdapter.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Helpers
{
    public static class KarmaPathUtils
    {
        public static bool IsSettingsFile(string path)
        {
            return PathUtils.PathHasFileName(path, Globals.SettingsFilename);
        }

        public static bool IsKarmaConfigFile(string path)
        {
            return PathUtils.PathHasFileName(path, Globals.KarmaConfigFilename);
        }
    }
}
