using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public static class XElementExtensions
    {
        public static string GetAttributeValue(this XElement element, XName name)
        {
            if (element == null)
            {
                return null;
            }
            var attribute = element.Attribute(name);
            return attribute == null ? null : attribute.Value;
        }

        public static IEnumerable<XElement> GetElements(this XElement element)
        {
            if (element == null)
            {
                return Enumerable.Empty<XElement>();
            }
            return element.Elements();
        }

        public static IEnumerable<XElement> GetElements(this XElement element, XName name)
        {
            if (element == null)
            {
                return Enumerable.Empty<XElement>();
            }
            return element.Elements(name);
        }

        public static XElement GetElement(this XElement element, string name)
        {
            if (element == null)
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                return element;
            }
            var names = name.Split('/');
            element = element.Element(names.First());
            return element == null ? null : element.GetElement(string.Join("/", names.Skip(1)));
        }

        public static string GetElementValue(this XElement element, string name = null)
        {
            element = element.GetElement(name);
            return element == null ? null : element.Value;
        }
    }
}
