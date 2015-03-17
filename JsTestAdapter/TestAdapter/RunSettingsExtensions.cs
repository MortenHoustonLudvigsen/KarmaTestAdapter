using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsTestAdapter.TestAdapter
{
    public static class RunSettingsExtensions
    {
        public static TestSettingsProvider GetTestSettingsProvider(this IRunSettings runSettings, string name)
        {
            return runSettings.GetSettings(name) as TestSettingsProvider;
        }

        public static TestSettings GetTestSettings(this IRunSettings runSettings, string name)
        {
            var provider = runSettings.GetTestSettingsProvider(name);
            if (provider == null)
            {
                return null;
            }
            return provider.Settings as TestSettings;
        }
    }
}