using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IOPath = System.IO.Path;

namespace KarmaTestAdapter.TestResults
{
    public class Source : Item
    {
        public Source(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public string Path { get { return Attribute("Path"); } }
        public int? Line { get { return Attribute("Line").ToInt(); } }
        public int? Column { get { return Attribute("Column").ToInt(); } }

        public string FullPath
        {
            get { return PathUtils.GetFullPath(Path, Root.KarmaConfig.BasePath); }
        }
    }
}
