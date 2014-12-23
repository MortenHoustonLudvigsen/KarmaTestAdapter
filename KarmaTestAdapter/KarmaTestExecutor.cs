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
using KarmaTestAdapter.Commands;
using Newtonsoft.Json;

namespace KarmaTestAdapter
{
    [ExtensionUri(Globals.ExecutorUriString)]
    public class KarmaTestExecutor : ITestExecutor
    {
        private KarmaDiscoverCommand _karmaDiscoverCommand;
        private KarmaRunCommand _karmaRunCommand;

        private bool _cancelled;
        private IKarmaLogger _currentLogger;

        public void Cancel()
        {
            _cancelled = true;
            if (_currentLogger != null)
            {
                if (_karmaDiscoverCommand != null)
                {
                    _karmaDiscoverCommand.Cancel(_currentLogger);
                }
                if (_karmaRunCommand != null)
                {
                    _karmaRunCommand.Cancel(_currentLogger);
                }
            }
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
            _currentLogger = logger;
            try
            {
                var karma = Discover(source, logger);
                if (karma == null)
                {
                    return;
                }

                if (tests == null)
                {
                    // Run all tests
                    tests = karma.GetTestCases(source);
                }

                var testsByName = tests
                    .GroupBy(t => t.DisplayName)
                    .Select(t => new
                    {
                        DisplayName = t.Key,
                        Tests = t.Select((tc, i) => new { Test = tc, Index = i })
                    })
                    .SelectMany(t => t.Tests)
                    .GroupBy(t => t.Index);

                foreach (var t in testsByName)
                {
                    RunTests(source, t.Select(x => x.Test), karma, frameworkHandle, logger);
                }
            }
            finally
            {
                _currentLogger = null;
            }
        }

        private void RunTests(string source, IEnumerable<TestCase> tests, KarmaTestResults.Karma karma, IFrameworkHandle frameworkHandle, IKarmaLogger logger)
        {
            var vsConfig = CreateVsConfig(tests, karma);
            var runKarma = Run(source, vsConfig, logger);
            if (runKarma == null)
            {
                logger.Error("No karma");
                return;
            }

            var consolidatedResults = runKarma.ConsolidateResults(logger);
            var testNames = tests.Select(t => t.DisplayName).Union(consolidatedResults.Select(r => r.Test.DisplayName));

            var results = from displayName in testNames
                          join test in tests
                            on displayName equals test.DisplayName
                            into test_
                          from test in test_.DefaultIfEmpty()
                          join result in consolidatedResults
                            on displayName equals result.Test.DisplayName
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

        private VsConfig.Config CreateVsConfig(IEnumerable<TestCase> tests, KarmaTestResults.Karma karma)
        {
            //var includedWithTests = tests.Select(t => t.CodeFilePath).Distinct();
            return new VsConfig.Config
            {
                files = karma.Files
                    //.Where(f => !f.AllTests.Any() || includedWithTests.Any(fi => f.HasFile(fi)))
                    .Select(f => new VsConfig.File
                    {
                        path = f.Path,
                        served = f.Served,
                        included = f.Included,
                        tests = f.AllTests
                            //.Where(t => tests.Any(tc => tc.FullyQualifiedName == t.FullyQualifiedName))
                            .Select(t => new VsConfig.Test
                            {
                                name = t.Name,
                                index = t.Index,
                                include = tests.Any(tc => tc.FullyQualifiedName == t.FullyQualifiedName)
                            })
                    })
            };
        }

        private KarmaTestResults.Karma Discover(string source, IKarmaLogger logger)
        {
            if (_karmaDiscoverCommand != null)
            {
                throw new Exception("Test discovery already running");
            }
            _karmaDiscoverCommand = new KarmaDiscoverCommand(source);
            try
            {
                return _karmaDiscoverCommand.Run(logger);
            }
            finally
            {
                _karmaDiscoverCommand = null;
            }
        }

        private KarmaTestResults.Karma Run(string source, VsConfig.Config vsConfig, IKarmaLogger logger)
        {
            if (_karmaRunCommand != null)
            {
                throw new Exception("Karma is already running");
            }
            _karmaRunCommand = new KarmaRunCommand(source, vsConfig);
            try
            {
                return _karmaRunCommand.Run(logger);
            }
            finally
            {
                _karmaRunCommand = null;
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
