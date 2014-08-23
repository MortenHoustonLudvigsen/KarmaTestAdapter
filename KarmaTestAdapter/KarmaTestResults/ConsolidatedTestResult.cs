using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.KarmaTestResults
{
    public class ConsolidatedTestResult
    {
        public ConsolidatedTestResult(Test test)
        {
            Test = test;
            Results = new List<TestResult>();
        }

        public Test Test { get; set; }
        public List<TestResult> Results { get; private set; }

        public TestOutcome Outcome
        {
            get
            {
                var result = TestOutcome.None;
                foreach (var res in Results)
                {
                    switch (res.Outcome)
                    {
                        case TestOutcome.Passed:
                            if (result == TestOutcome.None)
                            {
                                result = TestOutcome.Passed;
                            }
                            break;
                        case TestOutcome.Failed:
                            result = TestOutcome.Failed;
                            break;
                        case TestOutcome.Skipped:
                            if (result != TestOutcome.Failed)
                            {
                                result = TestOutcome.Skipped;
                            }
                            break;
                    }
                }
                return result;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                var result = TimeSpan.Zero;
                foreach (var res in Results)
                {
                    result += res.Time;
                }
                return result;
            }
        }
    }
}
