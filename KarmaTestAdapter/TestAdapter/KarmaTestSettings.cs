using JsTestAdapter.TestAdapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KarmaTestAdapter.TestAdapter
{
    [XmlType(KarmaTestSettings.SettingsName)]
    public class KarmaTestSettings : TestSettings
    {
        public const string SettingsName = "KarmaTestSettings";

        public KarmaTestSettings()
            : base(SettingsName)
        {
        }
    }
}
