using KarmaTestAdapter.Helpers;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

        public KarmaSourceSettings AddSource(string name, string source)
        {
            return AddSource(new KarmaSourceSettings { Name = name, Source = source });
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
        private static XmlSerializer _serializer = new XmlSerializer(typeof(KarmaSourceSettings));
        private static XmlSerializerNamespaces _serializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Source { get; set; }

        [XmlAttribute]
        public int Port { get; set; }

        public static string SettingsFilePath(string source)
        {
            return Path.Combine(Globals.GlobalLogDir, Sha1Utils.GetHash(source.ToLowerInvariant()) + ".xml");
        }

        public static KarmaSourceSettings Load(string source)
        {
            var i = 0;
            while (true)
            {
                try
                {
                    using (var reader = XmlReader.Create(SettingsFilePath(source)))
                    {
                        return _serializer.Deserialize(reader) as KarmaSourceSettings;
                    }
                }
                catch (Exception)
                {
                    i += 1;
                    if (i >= 10)
                    {
                        return null;
                    }
                    Thread.Sleep(100);
                }
            }
        }

        public void Save()
        {
            var saved = false;
            var i = 0;
            while (!saved)
            {
                try
                {
                    using (var writer = XmlWriter.Create(SettingsFilePath(Source)))
                    {
                        _serializer.Serialize(writer, this, _serializerNamespaces);
                    }
                    saved = true;
                }
                catch (Exception)
                {
                    i += 1;
                    if (i >= 10)
                    {
                        throw;
                    }
                    Thread.Sleep(100);
                }
            }
        }

        public void DeleteSettingsFile()
        {
            try
            {
                File.Delete(SettingsFilePath(Source));
            }
            catch
            {
                // Do nothing
            }
        }

        public override string ToString()
        {
            return string.Format("Port: {1} ({0})", Source, Port);
        }
    }
}