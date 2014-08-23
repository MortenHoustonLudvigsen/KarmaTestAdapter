using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.KarmaTestResults
{
    public class TestResult : Item
    {
        public TestResult(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public SuiteResult ParentSuite { get { return GetParent<SuiteResult>(); } }
        public int? Id { get { return Attribute("Id").ToInt(); } }
        public Browser Browser { get { return GetParent<Browser>(); } }
        public TimeSpan Time { get { return TimeSpan.FromMilliseconds(Math.Max(0.5, Attribute("Time").ToInt() ?? 0)); } }
        public TestOutcome Outcome { get { return GetTestOutcome(Attribute("Outcome")); } }
        public IEnumerable<string> Log { get { return Elements("Log").Select(e => e.Value); } }
        public string Message { get { return string.Join(Environment.NewLine, Log); } }

        public string DisplayName
        {
            get
            {
                return ParentSuite != null ? ParentSuite.DisplayName + " " + Name : Name;
            }
        }

        private TestOutcome GetTestOutcome(string outcome)
        {
            switch (outcome)
            {
                case "Success":
                    return TestOutcome.Passed;
                case "Skipped":
                    return TestOutcome.Skipped;
                case "Failed":
                    return TestOutcome.Failed;
                default:
                    return TestOutcome.None;
            }
        }
    }
}
