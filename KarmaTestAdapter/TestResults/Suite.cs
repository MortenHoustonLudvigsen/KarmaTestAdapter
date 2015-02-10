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
            Framework = Attribute("Framework");
            Line = Attribute("Line").ToInt();
            Column = Attribute("Column").ToInt();
        }

        public override bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public string Framework { get; private set; }
        public int? Line { get; private set; }
        public int? Column { get; private set; }

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
