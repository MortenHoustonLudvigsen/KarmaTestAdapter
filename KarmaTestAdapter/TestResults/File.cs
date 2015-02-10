using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IOPath = System.IO.Path;

namespace KarmaTestAdapter.TestResults
{
    public class File : TestCaseContainer
    {
        public File(Item parent, XElement element)
            : base(parent, element)
        {
            Path = Attribute("Path");
            Served = Attribute("Served").ToBool();
            Included = Attribute("Included").ToBool();
            Name = Path;
            FullPath = GetFullPath(Path);
        }

        public override bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(Path); }
        }

        public string Path { get; private set; }
        public bool? Served { get; private set; }
        public bool? Included { get; private set; }
        public string FullPath { get; private set; }

        public bool HasFile(string fullPath)
        {
            if (string.Equals(fullPath, FullPath, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return AllTests.Any(t => t.Source != null && string.Equals(fullPath, t.Source.FullPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}
