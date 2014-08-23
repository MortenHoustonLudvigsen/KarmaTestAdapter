using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public static class Globals
    {
        /// <summary>
        /// Should logging be done to a file as well as normal logging
        /// </summary>
        public const bool LogToFile = false;

        /// <summary>
        /// The file to log to
        /// </summary>
        public static string LogFilename
        {
            get { return Path.Combine(HomeDirectory, "KarmaTestAdapter.log"); }
        }

        /// <summary>
        /// The current user's home directory
        /// </summary>
        public static string HomeDirectory
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    return Environment.GetEnvironmentVariable("HOME");
                }
                else
                {
                    return Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
                }
            }
        }

        /// <summary>
        /// The Uri string used to identify the XmlTestExecutor.
        /// </summary>
        public const string ExecutorUriString = "executor://KarmaTestAdapterTestExecutor";

        /// <summary>
        /// The Uri used to identify the XmlTestExecutor.
        /// </summary>
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        /// <summary>
        /// The file name to log to
        /// </summary>
        public const string SettingsFilename = @"karma-vs-reporter.json";

        /// <summary>
        /// The file name to log to
        /// </summary>
        public const string KarmaSettingsFilename = @"karma.conf.js";

        /// <summary>
        /// Known file extensions
        /// </summary>
        public const string JavaScriptExtension = ".js";
        public const string TypeScriptExtension = ".ts";
        public const string TypeScriptDefExtension = ".d.ts";
        public const string CoffeeScriptExtension = ".coffee";
        public const string HtmScriptExtension = ".htm";
        public const string HtmlScriptExtension = ".html";
        public const string JsonExtension = ".json";
    }
}
