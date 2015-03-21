using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using JsTestAdapter.TestAdapter;
using JsTestAdapter.TestServerClient;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Karma;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestContainer : TestContainer
    {
        public KarmaTestContainer(TestContainerList containers, IVsProject project, string source)
            : base(containers, project, source)
        {
        }

        protected override void Init()
        {
            Settings = new KarmaSettings(Source, f => File.Exists(f), BaseDirectory, Logger);
            Validate(Settings.AreValid, Settings.InvalidReason);
            if (Settings.AreValid)
            {
                if (Settings.HasSettingsFile)
                {
                    Validate(Project.HasFile(Settings.SettingsFile), "File {1} is not included in project {0}", Project.GetProjectName(), GetRelativePath(Settings.SettingsFile));
                    Validate(Discoverer.ServiceProvider.HasFile(Settings.KarmaConfigFile), "File {0} is not included in solution", GetRelativePath(Settings.KarmaConfigFile));
                }
                else
                {
                    Validate(Project.HasFile(Settings.KarmaConfigFile), "File {1} is not included in project {0}", Project.GetProjectName(), GetRelativePath(Settings.KarmaConfigFile));
                }
            }

            if (Settings.Disabled)
            {
                Validate(false, string.Format("Karma is disabled in {0}", GetRelativePath(Settings.SettingsFile)));
            }
        }

        public KarmaSettings Settings { get; private set; }

        protected override TestServerLogger CreateServerLogger()
        {
            return new KarmaServerTestLogger(Logger);
        }

        protected override TestServer CreateTestServer()
        {
            return new KarmaServer(Settings, Logger);
        }

        public override bool HasFile(string file)
        {
            return PathUtils.PathsEqual(file, Settings.SettingsFile) || PathUtils.PathsEqual(file, Settings.KarmaConfigFile);
        }

        public override IEnumerable<string> GetFilesToWatch()
        {
            yield return Settings.SettingsFile;
            yield return Settings.KarmaConfigFile;
        }

        public override bool IsDuplicate(TestContainer other)
        {
            return base.IsDuplicate(other);
        }

        public override int Priority
        {
            get { return base.Priority; }
        }
    }
}