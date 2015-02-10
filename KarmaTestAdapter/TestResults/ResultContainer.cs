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
            Suites = GetChildren<SuiteResult>();
            TestResults = GetChildren<TestResult>();
            AllTestResults = GetAllChildren<TestResult>();
        }

        public virtual IEnumerable<SuiteResult> Suites { get; private set; }
        public virtual IEnumerable<TestResult> TestResults { get; private set; }

        [JsonIgnore]
        public virtual IEnumerable<TestResult> AllTestResults { get; private set; }
    }
}
