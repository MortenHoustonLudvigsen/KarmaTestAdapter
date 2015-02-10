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
        }

        public string Path { get; private set; }
        public bool? Served { get; private set; }
        public bool? Included { get; private set; }

        public override string Name { get { return Path; } }

        public string FullPath
        {
            get { return PathUtils.GetFullPath(Path, Root.KarmaConfig.BasePath); }
        }

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
