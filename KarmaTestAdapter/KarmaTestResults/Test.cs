using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.KarmaTestResults
{
    public class Test : Item
    {
        public Test(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public string Framework { get { return Attribute("Framework"); } }
        public int? Line { get { return Attribute("Line").ToInt(); } }
        public int? Column { get { return Attribute("Column").ToInt(); } }
        public Source Source { get { return Children.OfType<Source>().FirstOrDefault(); } }
        public File File { get { return GetParent<File>(); } }
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
                return File != null ? File.Path + " " + DisplayName : DisplayName;
            }
        }
    }
}
