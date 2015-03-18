using JsTestAdapter.Logging;
using JsTestAdapter.TestServerClient;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JsTestAdapter.TestAdapter
{
    public abstract class TestRunner : ITestDiscoverer, ITestExecutor
    {
        public abstract TestAdapterInfo CreateTestAdapterInfo();
        public TestAdapterInfo TestAdapterInfo { get; private set; }

        #region ITestDiscoverer

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            TestAdapterInfo = CreateTestAdapterInfo();
            var testLogger = TestAdapterInfo.CreateLogger(logger);
            var discoverLogger = new TestLogger(testLogger, "Discover");
            var testSettings = discoveryContext.RunSettings.GetTestSettings(TestAdapterInfo.SettingsName);
            foreach (var source in sources)
            {
                var sourceSettings = GetSourceSettings(source, testSettings);
                if (sourceSettings != null)
                {
                    DiscoverTests(sourceSettings, testLogger, discoverySink).Wait();
                }
                else
                {
                    discoverLogger.Warn("Could not get settings for {0}", source);
                }
            }
        }

        public TestSourceSettings GetSourceSettings(string source, TestSettings testSettings)
        {
            TestSourceSettings sourceSettings = null;
            if (testSettings != null)
            {
                sourceSettings = testSettings.GetSource(source);
            }
            if (sourceSettings == null)
            {
                sourceSettings = SourceSettingsPersister.Load(TestAdapterInfo.SettingsFileDirectory, source);
            }
            return sourceSettings;
        }

        private async Task DiscoverTests(TestSourceSettings settings, ITestLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            logger = new TestLogger(logger, settings.Name, "Discover");
            var tests = new ConcurrentBag<Guid>();
            if (settings.Port > 0)
            {
                logger.Info("Start");
                var discoverCommand = new DiscoverCommand(settings.Port);
                await discoverCommand.Run(spec =>
                {
                    var testCase = CreateTestCase(settings, spec);
                    tests.Add(testCase.Id);
                    discoverySink.SendTestCase(testCase);
                });
                await new RequestRunCommand(settings.Port).Run(tests);
                logger.Info("Complete");
            }
            else
            {
                logger.Error("Not connected to {0}", TestAdapterInfo.Name);
            }
        }

        public TestCase CreateTestCase(TestSourceSettings settings, Spec spec)
        {
            var fullyQualifiedName = string.Format("{0} / {1}", settings.Name, spec.UniqueName);
            var testCase = new TestCase(fullyQualifiedName, TestAdapterInfo.ExecutorUri, settings.Source);
            testCase.DisplayName = spec.Description;
            if (spec.Source != null)
            {
                testCase.CodeFilePath = spec.Source.FileName;
                if (spec.Source.LineNumber.HasValue)
                {
                    testCase.LineNumber = spec.Source.LineNumber.Value;
                }
            }
            return testCase;
        }

        #endregion

        #region ITestExecutor

        public void Cancel()
        {
            // Can not cancel
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            TestAdapterInfo = CreateTestAdapterInfo();

            var testLogger = TestAdapterInfo.CreateLogger(frameworkHandle);
            var runLogger = new TestLogger(testLogger, "Run");
            var testSettings = runContext.RunSettings.GetTestSettings(TestAdapterInfo.SettingsName);

            foreach (var source in sources)
            {
                var sourceSettings = GetSourceSettings(source, testSettings);
                if (sourceSettings != null)
                {
                    RunTests(sourceSettings, testLogger, runContext, frameworkHandle).Wait();
                }
                else
                {
                    runLogger.Warn("Could not get settings for {0}", source);
                }
            }
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            RunTests(tests.Select(t => t.Source).Distinct(), runContext, frameworkHandle);
        }

        private async Task RunTests(TestSourceSettings settings, ITestLogger logger, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            logger = new TestLogger(logger, settings.Name, "Run");
            if (settings.Port > 0)
            {
                logger.Info("Start");
                var discoverCommand = new DiscoverCommand(settings.Port);
                await discoverCommand.Run(spec => RunTest(settings, logger, runContext, frameworkHandle, spec));
                logger.Info("Complete");
            }
            else
            {
                logger.Error("Not connected to {0}", TestAdapterInfo.Name);
            }
        }

        private void RunTest(TestSourceSettings settings, ITestLogger logger, IRunContext runContext, IFrameworkHandle frameworkHandle, Spec spec)
        {
            var testCase = CreateTestCase(settings, spec);
            var outcome = TestOutcome.None;

            frameworkHandle.RecordStart(testCase);
            foreach (var result in spec.Results)
            {
                if (result.Skipped && outcome != TestOutcome.Failed)
                {
                    outcome = TestOutcome.Skipped;
                }

                if (result.Success && outcome == TestOutcome.None)
                {
                    outcome = TestOutcome.Passed;
                }

                if (!result.Success && !result.Skipped)
                {
                    outcome = TestOutcome.Failed;
                }

                var testResult = new TestResult(testCase)
                {
                    ComputerName = Environment.MachineName,
                    DisplayName = result.Name,
                    Outcome = GetTestOutcome(result),
                    Duration = TimeSpan.FromTicks(Math.Max(Convert.ToInt64((result.Time ?? 0) * TimeSpan.TicksPerMillisecond), 1))
                };

                if (result.Failures != null && result.Failures.Any())
                {
                    var failure = result.Failures.First();
                    testResult.ErrorMessage = failure.message;
                    testResult.ErrorStackTrace = string.Join(Environment.NewLine, failure.stack);
                    foreach (var extraFailure in result.Failures.Skip(1))
                    {
                        testResult.Messages.Add(new TestResultMessage(TestResultMessage.AdditionalInfoCategory,
                            string.Join(Environment.NewLine, extraFailure.message, string.Join(Environment.NewLine, extraFailure.stack))
                        ));
                    }
                    if (result.Log != null && result.Log.Any())
                    {
                        testResult.Messages.Add(new TestResultMessage(TestResultMessage.StandardErrorCategory, string.Join(Environment.NewLine, result.Log)));
                    }
                }
                else if (result.Log != null && result.Log.Any())
                {
                    testResult.Messages.Add(new TestResultMessage(TestResultMessage.StandardErrorCategory, string.Join(Environment.NewLine, result.Log)));
                    testResult.ErrorMessage = string.Join(Environment.NewLine, result.Log);
                }

                if (!string.IsNullOrWhiteSpace(result.Output))
                {
                    testResult.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, result.Output));
                }

                frameworkHandle.RecordResult(testResult);
            }
            frameworkHandle.RecordEnd(testCase, outcome);
        }

        private static TestOutcome GetTestOutcome(SpecResult result)
        {
            if (result.Skipped)
            {
                return TestOutcome.Skipped;
            }
            return result.Success ? TestOutcome.Passed : TestOutcome.Failed;
        }
        
        #endregion
    }
}