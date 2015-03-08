using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace KarmaTestAdapter.TestAdapter
{
    [XmlType(KarmaTestSettings.SettingsName)]
    public class KarmaTestSettings : TestRunSettings
    {
        public const string SettingsName = "KarmaTestSettings";

        public KarmaTestSettings()
            : base(SettingsName)
        {
            Sources = new List<KarmaSourceSettings>();
        }

        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(KarmaTestSettings));
        private static readonly XmlSerializerNamespaces _serializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

        public List<KarmaSourceSettings> Sources { get; set; }

        public KarmaSourceSettings AddSource(string source)
        {
            return AddSource(new KarmaSourceSettings { Source = source });
        }

        private KarmaSourceSettings AddSource(KarmaSourceSettings source)
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

        public KarmaSourceSettings GetSource(string source)
        {
            return Sources.FirstOrDefault(s => s.Source.Equals(source, StringComparison.OrdinalIgnoreCase));
        }

        public override XmlElement ToXml()
        {
            var document = new XmlDocument();
            document.LoadXml(Serialize());
            return document.DocumentElement;
        }

        public static KarmaTestSettings Deserialize(XmlReader reader)
        {
            return _serializer.Deserialize(reader) as KarmaTestSettings;
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

    [XmlType("Source")]
    public class KarmaSourceSettings
    {
        [XmlAttribute]
        public string BaseDirectory { get; set; }

        [XmlAttribute]
        public string Source { get; set; }

        [XmlAttribute]
        public int Port { get; set; }

        public override string ToString()
        {
            return string.Format("Port: {1} ({0})", Source, Port);
        }
    }
}