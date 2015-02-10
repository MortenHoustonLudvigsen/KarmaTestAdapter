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
    public class TestResult : Item
    {
        public TestResult(Item parent, XElement element)
            : base(parent, element)
        {
            Id = Attribute("Id").ToInt();
            Time = TimeSpan.FromMilliseconds(Math.Max(0.5, Attribute("Time").ToInt() ?? 0));
            Outcome = GetTestOutcome(Attribute("Outcome"));
            Log = Elements("Log").Select(e => e.Value);
            Message = string.Join(Environment.NewLine, Log);
            ParentSuite = GetParent<SuiteResult>();
            Browser = GetParent<Browser>();
        }

        public override bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }

        public int? Id { get; private set; }
        public TimeSpan Time { get; private set; }
        public TestOutcome Outcome { get; private set; }
        public IEnumerable<string> Log { get; private set; }
        public string Message { get; private set; }

        [JsonIgnore]
        public SuiteResult ParentSuite { get; private set; }

        [JsonIgnore]
        public Browser Browser { get; private set; }

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
