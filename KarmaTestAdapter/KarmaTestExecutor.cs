using KarmaTestResults = KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.TestWindow.Extensibility;

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
            _cancelled = false;

            foreach (var source in sources)
            {
                RunTests(source, null, runContext, frameworkHandle);
            }
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            _cancelled = false;

            foreach (var g in tests.GroupBy(t => t.Source))
            {
                RunTests(g.Key, g, runContext, frameworkHandle);
            }
        }

        public void RunTests(string source, IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var logger = KarmaLogger.Create(messageLogger: frameworkHandle);
            var karma = KarmaReporter.Discover(source, logger);
            VsConfig.Config vsConfig = new VsConfig.Config();

            if (tests == null)
            {
                // Run all tests
                logger.Info("Running all tests");
                tests = karma.GetTestCases(source);
            }
            else
            {
                logger.Info("Running some tests");
                var includedWithTests = tests.Select(t => t.CodeFilePath).Distinct();
                logger.Info("Files: " + string.Join(", ", includedWithTests));
                vsConfig.files = karma.Files
                    .Where(f => !f.AllTests.Any() || includedWithTests.Any(fi => f.HasFile(fi)))
                    .Select(f => new VsConfig.File
                    {
                        path = f.Path,
                        served = f.Served,
                        included = f.Included,
                        tests = f.AllTests
                            .Where(t => tests.Any(tc => tc.FullyQualifiedName == t.FullyQualifiedName))
                            .Select(t => new VsConfig.Test
                            {
                                name = t.Name,
                                index = t.Index
                            })
                    });
            }

            var testCases = tests.ToDictionary(t => t.FullyQualifiedName, t => t);

            karma = KarmaReporter.Run(source, vsConfig, logger);
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
                          select new TestCaseResult(test, result, source);

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

        private class TestCaseResult
        {
            public TestCaseResult(TestCase test, KarmaTestResults.ConsolidatedTestResult result, string source)
            {
                Result = result ?? new KarmaTestResults.ConsolidatedTestResult(null);
                Test = test ?? KarmaTestResults.Karma.CreateTestCase(result.Test, source);
            }

            public TestCase Test { get; private set; }
            public KarmaTestResults.ConsolidatedTestResult Result { get; private set; }
        }
    }
}
