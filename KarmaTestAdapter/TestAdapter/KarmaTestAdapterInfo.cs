using JsTestAdapter.Logging;
using JsTestAdapter.TestAdapter;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestAdapterInfo : TestAdapterInfo
    {
        public override string Name
        {
            get { return "Karma"; }
        }

        public override Uri ExecutorUri
        {
            get { return Globals.ExecutorUri; }
        }

        public override bool IsTestContainer(string file)
        {
            return KarmaPathUtils.IsSettingsFile(file) || KarmaPathUtils.IsKarmaConfigFile(file);
        }

        public override int GetContainerPriority(string file)
        {
            if (KarmaPathUtils.IsSettingsFile(file))
            {
                return 2;
            }
            else if (KarmaPathUtils.IsKarmaConfigFile(file))
            {
                return 1;
            }
            return 0;
        }

        public override string SettingsName
        {
            get { return KarmaTestSettings.SettingsName; }
        }

        public override string SettingsFileDirectory
        {
            get { return Globals.GlobalLogDir; }
        }

        public override ITestLogger CreateLogger(IMessageLogger logger)
        {
            return new KarmaLogger(logger);
        }

        public override ITestLogger CreateLogger(ILogger logger)
        {
            return new KarmaLogger(logger);
        }
    }
}
