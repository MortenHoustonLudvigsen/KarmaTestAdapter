using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace JsTestAdapter.TestAdapter.TestSettings
{
    [XmlType("Source")]
    public class SourceSettings
    {
        [XmlAttribute]
        public string Name { get; set; }

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