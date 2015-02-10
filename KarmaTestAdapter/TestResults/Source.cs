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
            Path = Attribute("Path");
            Line = Attribute("Line").ToInt();
            Column = Attribute("Column").ToInt();
            FullPath = GetFullPath(Path);
        }

        public override bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(Path); }
        }

        public string Path { get; private set; }
        public int? Line { get; private set; }
        public int? Column { get; private set; }
        public string FullPath { get; private set; }
    }
}
