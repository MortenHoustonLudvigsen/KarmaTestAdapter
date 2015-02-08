using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IOPath = System.IO.Path;

namespace KarmaTestAdapter.TestResults
{
    public class Suite : TestCaseContainer
    {
        public Suite(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public string Framework { get { return Attribute("Framework"); } }
        public int? Line { get { return Attribute("Line").ToInt(); } }
        public int? Column { get { return Attribute("Column").ToInt(); } }
        public Source Source { get { return Children.OfType<Source>().FirstOrDefault(); } }

        [JsonIgnore]
        public File File { get { return GetParent<File>(); } }

        [JsonIgnore]
        public Suite ParentSuite { get { return GetParent<Suite>(); } }

        public string DisplayName
        {
            get
            {
                return ParentSuite != null ? ParentSuite.DisplayName + " " + Name : Name;
            }
        }

        public string FullyQualifiedName
        {
            get
            {
                var fullyQualifiedName = Name.Replace('.', '-');
                if (ParentSuite != null)
                {
                    fullyQualifiedName = ParentSuite.FullyQualifiedName + " / " + fullyQualifiedName;
                }
                else
                {
                    var path = IOPath.ChangeExtension(Source != null ? Source.Path : File.Path, ".");
                    path = path.Replace('\\', '.');
                    path = path.Replace('/', '.');
                    fullyQualifiedName = path + fullyQualifiedName;
                }
                return fullyQualifiedName;
            }
        }
    }
}
