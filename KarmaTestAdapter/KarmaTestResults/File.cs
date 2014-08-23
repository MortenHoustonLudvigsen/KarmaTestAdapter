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
        public override string Name { get { return Path; } }

        public string FullPath
        {
            get { return IOPath.GetFullPath(IOPath.Combine(Root.KarmaConfig.BasePath, Path)); }
        }
    }
}
