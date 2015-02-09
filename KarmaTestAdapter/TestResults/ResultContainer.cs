using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public abstract class ResultContainer : Item
    {
        public ResultContainer(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public virtual IEnumerable<SuiteResult> Suites { get { return GetChildren<SuiteResult>(); } }
        public virtual IEnumerable<TestResult> TestResults { get { return GetChildren<TestResult>(); } }

        [JsonIgnore]
        public virtual IEnumerable<TestResult> AllTestResults { get { return GetAllChildren<TestResult>(); } }
    }
}
