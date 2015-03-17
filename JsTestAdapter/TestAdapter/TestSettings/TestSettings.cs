using JsTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace JsTestAdapter.TestAdapter.TestSettings
{
    public abstract class TestSettings<TTestSettings> : TestRunSettings
        where TTestSettings : TestSettings<TTestSettings>, new()
    {
        public TestSettings(string settingsName)
            : base(settingsName)
        {
            Sources = new List<SourceSettings>();
        }

        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(TTestSettings));
        private static readonly XmlSerializerNamespaces _serializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

        public List<SourceSettings> Sources { get; set; }

        public SourceSettings AddSource(string name, string source)
        {
            return AddSource(new SourceSettings
            {
                Name = name,
                Source = source
            });
        }

        private SourceSettings AddSource(SourceSettings source)
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

        public SourceSettings GetSource(string source)
        {
            return Sources.FirstOrDefault(s => s.Source.Equals(source, StringComparison.OrdinalIgnoreCase));
        }

        public override XmlElement ToXml()
        {
            var document = new XmlDocument();
            document.LoadXml(Serialize());
            return document.DocumentElement;
        }

        public static TTestSettings Deserialize(XmlReader reader)
        {
            return _serializer.Deserialize(reader) as TTestSettings;
        }

        public string Serialize()
        {
            using (var writer = new StringWriter())
            {
                _serializer.Serialize(writer, this, _serializerNamespaces);
                return writer.ToString();
            }
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}