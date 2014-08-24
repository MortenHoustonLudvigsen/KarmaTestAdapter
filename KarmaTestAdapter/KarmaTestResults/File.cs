using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IOPath = System.IO.Path;

namespace KarmaTestAdapter.KarmaTestResults
{
    public class File : TestCaseContainer
    {
        public File(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public string Path { get { return Attribute("Path"); } }
        public bool? Served { get { return Attribute("Served").ToBool(); } }
        public bool? Included { get { return Attribute("Included").ToBool(); } }

        public override string Name { get { return Path; } }

        public string FullPath
        {
            get { return IOPath.GetFullPath(IOPath.Combine(Root.KarmaConfig.BasePath, Path)); }
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
