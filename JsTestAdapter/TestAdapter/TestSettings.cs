using JsTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace JsTestAdapter.TestAdapter
{
    public abstract class TestSettings : TestRunSettings
    {
        public TestSettings(string settingsName)
            : base(settingsName)
        {
            Sources = new List<TestSourceSettings>();
        }

        public List<TestSourceSettings> Sources { get; set; }

        public TestSourceSettings AddSource(string name, string source)
        {
            return AddSource(new TestSourceSettings
            {
                Name = name,
                Source = source
            });
        }

        private TestSourceSettings AddSource(TestSourceSettings source)
        {
            if (source != null)
            {
                RemoveSource(source.Source);
                Sources.Add(source);
            }
            return source;
        }

        public void RemoveSource(string source)
        {
            Sources = Sources.Where(s => !s.Source.Equals(source, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public TestSourceSettings GetSource(string source)
        {
            return Sources.FirstOrDefault(s => s.Source.Equals(source, StringComparison.OrdinalIgnoreCase));
        }

        public override XmlElement ToXml()
        {
            var document = new XmlDocument();
            document.LoadXml(Serialize());
            return document.DocumentElement;
        }

        public string Serialize()
        {
            return XmlSerialization.Serialize(this);
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}