using JsTestAdapter.TestAdapter.TestSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KarmaTestAdapter.TestAdapter.TestSettings
{
    [XmlType(KarmaTestSettings.SettingsName)]
    public class KarmaTestSettings : TestSettings<KarmaTestSettings>
    {
        public const string SettingsName = "KarmaTestSettings";

        public KarmaTestSettings()
            : base(SettingsName)
        {
        }
    }
}
