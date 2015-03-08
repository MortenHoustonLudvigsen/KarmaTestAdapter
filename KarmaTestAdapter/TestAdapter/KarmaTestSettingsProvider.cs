using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.XPath;

namespace KarmaTestAdapter.TestAdapter
{
    [Export(typeof(ISettingsProvider))]
    [Export(typeof(IRunSettingsService))]
    [Export(typeof(KarmaTestSettingsProvider))]
    [SettingsName(KarmaTestSettings.SettingsName)]
    public class KarmaTestSettingsProvider : IRunSettingsService, ISettingsProvider
    {
        public KarmaTestSettingsProvider()
        {
            Settings = new KarmaTestSettings();
        }

        public IXPathNavigable AddRunSettings(IXPathNavigable inputRunSettingDocument, IRunSettingsConfigurationInfo configurationInfo, ILogger log)
        {
            ValidateArg.NotNull(inputRunSettingDocument, "inputRunSettingDocument");
            ValidateArg.NotNull(configurationInfo, "configurationInfo");

            var navigator = inputRunSettingDocument.CreateNavigator();
            if (navigator.MoveToChild("RunSettings", ""))
            {
                if (navigator.MoveToChild(KarmaTestSettings.SettingsName, ""))
                {
                    navigator.DeleteSelf();
                }
                navigator.AppendChild(Settings.Serialize());
            }

            navigator.MoveToRoot();
            return navigator;
        }

        public string Name { get { return KarmaTestSettings.SettingsName; } }
        public KarmaTestSettings Settings { get; private set; }

        public void Load(XmlReader reader)
        {
            ValidateArg.NotNull(reader, "reader");

            if (reader.Read() && reader.Name.Equals(KarmaTestSettings.SettingsName))
            {
                Settings = KarmaTestSettings.Deserialize(reader);
            }
        }
    }
}