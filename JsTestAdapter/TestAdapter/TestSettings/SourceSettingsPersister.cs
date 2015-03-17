using JsTestAdapter.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace JsTestAdapter.TestAdapter.TestSettings
{
    public class SourceSettingsPersister
    {
        public static SourceSettings Load(string settingsFileDirectory, string source)
        {
            return new SourceSettingsPersister(settingsFileDirectory).Load(source);
        }

        public static void Save(string settingsFileDirectory, SourceSettings sourceSettings)
        {
            new SourceSettingsPersister(settingsFileDirectory).Save(sourceSettings);
        }

        public static void DeleteSettingsFile(string settingsFileDirectory, SourceSettings sourceSettings)
        {
            new SourceSettingsPersister(settingsFileDirectory).DeleteSettingsFile(sourceSettings);
        }

        public SourceSettingsPersister(string settingsFileDirectory)
        {
            _settingsFileDirectory = settingsFileDirectory;
        }

        private static XmlSerializer _serializer = new XmlSerializer(typeof(SourceSettings));
        private static XmlSerializerNamespaces _serializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

        private string _settingsFileDirectory;
        private string SettingsFilePath(string source)
        {
            return Path.Combine(_settingsFileDirectory, Sha1Utils.GetHash(source.ToLowerInvariant()) + ".xml");
        }

        private SourceSettings Load(string source)
        {
            var i = 0;
            while (true)
            {
                try
                {
                    using (var reader = XmlReader.Create(SettingsFilePath(source)))
                    {
                        return _serializer.Deserialize(reader) as SourceSettings;
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

        private void Save(SourceSettings sourceSettings)
        {
            var saved = false;
            var i = 0;
            while (!saved)
            {
                try
                {
                    using (var writer = XmlWriter.Create(SettingsFilePath(sourceSettings.Source)))
                    {
                        _serializer.Serialize(writer, sourceSettings, _serializerNamespaces);
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

        private void DeleteSettingsFile(SourceSettings sourceSettings)
        {
            try
            {
                File.Delete(SettingsFilePath(sourceSettings.Source));
            }
            catch
            {
                // Do nothing
            }
        }
    }
}