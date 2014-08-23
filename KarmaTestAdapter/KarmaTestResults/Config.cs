using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.KarmaTestResults
{
    public class Config : Item
    {
        public Config(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public string BasePath { get { return ValueOfElement("basePath"); } }
        public int? Port { get { return ValueOfElement("port").ToInt(); } }
        public IEnumerable<string> Frameworks
        {
            get
            {
                return Elements("frameworks")
                    .SelectMany(f => f.Elements("item").Select(e => e.Value));
            }
        }
    }
}
