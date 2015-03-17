using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using JsTestAdapter.TestServer;
using KarmaTestAdapter.TestAdapter;
using System;
using System.IO;
using TwoPS.Processes;

namespace KarmaTestAdapter.Karma
{
    public class KarmaServer : TestServer
    {
        public KarmaServer(KarmaSettings settings, ITestLogger logger)
            : base(logger)
        {
            if (!settings.AreValid)
            {
                throw new ArgumentException("Settings are not valid", "settings");
            }
            Settings = settings;
        }

        public KarmaSettings Settings { get; private set; }

        public override string Name
        {
            get { return "Karma"; }
        }

        public override string StartScript
        {
            get { return Path.Combine(Globals.LibDirectory, "Start.js"); }
        }

        public override string WorkingDirectory
        {
            get { return Path.GetDirectoryName(Settings.KarmaConfigFile); }
        }

        protected override void AddOptions(ProcessOptions options)
        {
            options.Add("--karma", PathUtils.GetRelativePath(WorkingDirectory, Settings.KarmaConfigFile));
            if (Settings.HasSettingsFile && File.Exists(Settings.SettingsFile))
            {
                options.Add("--settings", PathUtils.GetRelativePath(WorkingDirectory, Settings.SettingsFile));
            }
        }
    }
}