using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.KarmaTestResults
{
    public class SuiteResult : ResultContainer
    {
        public SuiteResult(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public SuiteResult ParentSuite { get { return GetParent<SuiteResult>(); } }

        public string DisplayName
        {
            get
            {
                return ParentSuite != null ? ParentSuite.DisplayName + " " + Name : Name;
            }
        }
    }
}
