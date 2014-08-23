using KarmaTestResults = KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    [ExtensionUri(Globals.ExecutorUriString)]
    public class KarmaTestExecutor : ITestExecutor
    {
        private bool _cancelled;

        public void Cancel()
        {
            _cancelled = true;
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            RunTests(KarmaTestDiscoverer.GetTests(sources, runContext, KarmaLogger.Create(messageLogger: frameworkHandle)), runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var logger = KarmaLogger.Create(messageLogger: frameworkHandle);
            _cancelled = false;

            foreach (var g in tests.GroupBy(t => t.Source))
            {
                var testCases = g.ToDictionary(t => t.FullyQualifiedName, t => t);
                var karma = KarmaReporter.Run(g.Key, logger);
                var consolidatedResults = karma.ConsolidateResults();
                var testNames = tests.Select(t => t.FullyQualifiedName).Union(consolidatedResults.Select(r => r.Test.FullyQualifiedName));

                var results = from fullyQualifiedName in testNames
                              join test in tests
                                on fullyQualifiedName equals test.FullyQualifiedName
                                into test_
                                from test in test_.DefaultIfEmpty()
                              join result in consolidatedResults
                                on fullyQualifiedName equals result.Test.FullyQualifiedName
                                into result_
                                from result in result_.DefaultIfEmpty()
                              select new TestCaseResult(test, result, g.Key);

                foreach (var result in results)
                {
                    frameworkHandle.RecordStart(result.Test);
                    foreach (var res in result.Result.Results)
                    {
                        frameworkHandle.RecordResult(new TestResult(result.Test)
                        {
                            ComputerName = Environment.MachineName,
                            DisplayName = res.Browser.Name,
                            Outcome = res.Outcome,
                            Duration = res.Time,
                            ErrorMessage = res.Message
                        });
                    }
                    frameworkHandle.RecordEnd(result.Test, result.Result.Outcome);
                }
            }
        }

        private class TestCaseResult
        {
            public TestCaseResult(TestCase test, KarmaTestResults.ConsolidatedTestResult result, string source)
            {
                Result = result ?? new KarmaTestResults.ConsolidatedTestResult(null);
                Test = test ?? KarmaTestDiscoverer.CreateTestCase(result.Test, source);
            }

            public TestCase Test { get; private set; }
            public KarmaTestResults.ConsolidatedTestResult Result { get; private set; }
        }
    }
}
