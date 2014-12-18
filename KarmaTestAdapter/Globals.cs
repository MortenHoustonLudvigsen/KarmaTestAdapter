using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
        public static readonly TestProperty FileIndexProperty = TestProperty.Register("KarmaTestCaseFileIndex", "Index in file", typeof(int?), typeof(KarmaTestDiscoverer));

        /// <summary>
        /// The file to log to when Settings.LogToFile == true
        /// </summary>
        public const string LogFilename = "KarmaTestAdapter.log";

        /// <summary>
        /// The file for karma output when Settings.LogToFile == true
        /// </summary>
        public const string OutputFilename = "KarmaTestAdapter.output.xml";

        /// <summary>
        /// The file for VsConfig when Settings.LogToFile == true
        /// </summary>
        public const string VsConfigFilename = "KarmaTestAdapter.VsConfig.json";

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
