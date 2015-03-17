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

namespace JsTestAdapter.TestAdapter.TestSettings
{
    public class TestSettingsProvider<TTestSettings> : IRunSettingsService, ISettingsProvider
        where TTestSettings : TestSettings<TTestSettings>, new()
    {
        public TestSettingsProvider()
        {
            Settings = new TTestSettings();
        }

        public IXPathNavigable AddRunSettings(IXPathNavigable inputRunSettingDocument, IRunSettingsConfigurationInfo configurationInfo, ILogger log)
        {
            ValidateArg.NotNull(inputRunSettingDocument, "inputRunSettingDocument");
            ValidateArg.NotNull(configurationInfo, "configurationInfo");

            var navigator = inputRunSettingDocument.CreateNavigator();
            if (navigator.MoveToChild("RunSettings", ""))
            {
                if (navigator.MoveToChild(Name, ""))
                {
                    navigator.DeleteSelf();
                }
                navigator.AppendChild(Settings.Serialize());
            }

            navigator.MoveToRoot();
            return navigator;
        }

        public string Name { get { return Settings.Name; } }
        public TTestSettings Settings { get; private set; }

        public void Load(XmlReader reader)
        {
            ValidateArg.NotNull(reader, "reader");

            if (reader.Read() && reader.Name.Equals(Name))
            {
                Settings = TestSettings<TTestSettings>.Deserialize(reader);
            }
        }
    }
}