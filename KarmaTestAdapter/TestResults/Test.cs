using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public class Test : TestItem
    {
        public Test(Item parent, XElement element)
            : base(parent, element)
        {
            Framework = Attribute("Framework");
            Line = Attribute("Line").ToInt();
            Column = Attribute("Column").ToInt();
            Index = Attribute("Index").ToInt();
        }

        public override bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public string Framework { get; private set; }
        public int? Line { get; private set; }
        public int? Column { get; private set; }
        public int? Index { get; private set; }

        public string FullyQualifiedName
        {
            get
            {
                // We need Index in the fully qualified name to differentiate between tests with identical names
                var fullyQualifiedName = string.Format("{0}#{1}", Name.Replace('.', '-'), Index);
                if (ParentSuite != null)
                {
                    fullyQualifiedName = ParentSuite.FullyQualifiedName + "." + fullyQualifiedName;
                }
                return fullyQualifiedName;
            }
        }
    }
}
