using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.TestAdapter.TestSettings
{
    public static class RunSettingsExtensions
    {
        public static KarmaTestSettingsProvider GetKarmaTestSettingsProvider(this IRunSettings runSettings)
        {
            return runSettings.GetSettings(KarmaTestSettings.SettingsName) as KarmaTestSettingsProvider;
        }

        public static KarmaTestSettings GetKarmaTestSettings(this IRunSettings runSettings)
        {
            var provider = runSettings.GetKarmaTestSettingsProvider();
            if (provider == null)
            {
                return null;
            }
            return provider.Settings;
        }
    }
}