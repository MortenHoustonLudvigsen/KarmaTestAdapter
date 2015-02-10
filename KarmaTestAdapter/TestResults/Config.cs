using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public class Config : Item
    {
        public Config(Item parent, XElement element)
            : base(parent, element)
        {
            BasePath = ValueOfElement("basePath");
            Port = ValueOfElement("port").ToInt();
            Frameworks = Elements("frameworks").SelectMany(f => f.Elements("item").Select(e => e.Value));
        }

        public override bool IsValid
        {
            get { return true; }
        }

        public string BasePath { get; private set; }
        public int? Port { get; private set; }
        public IEnumerable<string> Frameworks { get; private set; }
    }
}
