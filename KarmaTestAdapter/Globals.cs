using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace KarmaTestAdapter
{
    public static class Globals
    {
        /// <summary>
        /// The Uri string used to identify the KarmaTestAdapter executor.
        /// </summary>
        public const string ExecutorUriString = "executor://KarmaTestAdapter";

        /// <summary>
        /// The Uri used to identify the KarmaTestAdapter executor.
        /// </summary>
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        /// <summary>
        /// The file name for KarmaTestAdapter settings files
        /// </summary>
        public const string SettingsFilename = @"KarmaTestAdapter.json";

        /// <summary>
        /// The standard file name for the karma configuration file
        /// </summary>
        public const string KarmaConfigFilename = @"karma.conf.js";

        /// <summary>
        /// The file to log to when Settings.LogToFile == true
        /// </summary>
        public const string LogFilename = "KarmaTestAdapter.log";

        /// <summary>
        /// Whether to log to a global log file
        /// </summary>
        public const bool Debug = true;

        /// <summary>
        /// The global log directory
        /// </summary>
        public static readonly string GlobalLogDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "2PS", "KarmaTestAdapter");

        /// <summary>
        /// The global log directory
        /// </summary>
        public static readonly string GlobalLogFile = Path.Combine(GlobalLogDir, LogFilename);

        /// <summary>
        /// Indicates whether we are running automated tests
        /// </summary>
        public static bool IsTest = false;

        /// <summary>
        /// The directory in which this assembly resides
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetFullPath(Path.GetDirectoryName(path));
            }
        }

        /// <summary>
        /// The directory from which to find node modules and KarmaTestAdapter javascript files
        /// </summary>
        public static string RootDirectory
        {
            //get { return Path.GetFullPath(IsTest ? Path.Combine(AssemblyDirectory, @"..\..\..\KarmaTestAdapter\build") : AssemblyDirectory); }
            get { return Path.GetFullPath(IsTest ? Path.Combine(AssemblyDirectory, @"..\..\..\KarmaTestAdapter") : AssemblyDirectory); }
        }

        /// <summary>
        /// The directory from which to find KarmaTestAdapter javascript files
        /// </summary>
        public static string LibDirectory
        {
            get { return Path.GetFullPath(Path.Combine(RootDirectory, "lib")); }
        }
    }
}