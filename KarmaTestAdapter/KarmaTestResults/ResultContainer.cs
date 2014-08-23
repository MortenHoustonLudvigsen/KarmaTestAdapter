using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.KarmaTestResults
{
    public abstract class ResultContainer : Item
    {
        public ResultContainer(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public virtual IEnumerable<SuiteResult> Suites { get { return Children.OfType<SuiteResult>(); } }
        public virtual IEnumerable<TestResult> TestResults { get { return Children.OfType<TestResult>(); } }
        public virtual IEnumerable<TestResult> AllTestResults { get { return AllChildren.OfType<TestResult>(); } }
    }
}
