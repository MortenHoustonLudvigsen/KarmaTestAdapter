using JsTestAdapter.TestAdapter.TestSettings;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.TestAdapter.TestSettings
{
    [Export(typeof(ISettingsProvider))]
    [Export(typeof(IRunSettingsService))]
    [Export(typeof(KarmaTestSettingsProvider))]
    [SettingsName(KarmaTestSettings.SettingsName)]
    public class KarmaTestSettingsProvider : TestSettingsProvider<KarmaTestSettings>
    {
    }
}
