using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace JsTestAdapter.Helpers
{
    public static class XmlSerialization
    {
        private static readonly XmlSerializerNamespaces _serializerNamespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

        public static string Serialize(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj, _serializerNamespaces);
                return writer.ToString();
            }
        }

        public static T Deserialize<T>(XmlReader reader)
            where T: class
        {
            var serializer = new XmlSerializer(typeof(T));
            return serializer.Deserialize(reader) as T;
        }

        public static object Deserialize(Type type, XmlReader reader)
        {
            var serializer = new XmlSerializer(type);
            return serializer.Deserialize(reader);
        }
    }
}