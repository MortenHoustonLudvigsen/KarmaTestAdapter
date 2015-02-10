using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public class SuiteResult : ResultContainer
    {
        public SuiteResult(Item parent, XElement element)
            : base(parent, element)
        {
        }

        [JsonIgnore]
        public SuiteResult ParentSuite { get { return GetParent<SuiteResult>(); } }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return null;
                }
                return ParentSuite != null ? ParentSuite.DisplayName + " " + Name : Name;
            }
        }
    }
}
