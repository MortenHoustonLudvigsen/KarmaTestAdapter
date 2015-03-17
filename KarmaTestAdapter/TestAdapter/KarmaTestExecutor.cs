using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Karma;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    [ExtensionUri(Globals.ExecutorUriString)]
    public class KarmaTestExecutor : ITestExecutor
    {
        public void Cancel()
        {
            // Can not cancel
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var karmaLogger = new TestLogger(frameworkHandle);
            var runLogger = new TestLogger(karmaLogger, "Run");
            var testSettings = runContext.RunSettings.GetKarmaTestSettings();

            foreach (var source in sources)
            {
                var sourceSettings = KarmaTestDiscoverer.GetKarmaSourceSettings(source, testSettings);
                if (sourceSettings != null)
                {
                    RunTests(sourceSettings, karmaLogger, runContext, frameworkHandle).Wait();
                }
                else
                {
                    runLogger.Warn("Could not get karma settings for {0}", source);
                }
            }
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            RunTests(tests.Select(t => t.Source).Distinct(), runContext, frameworkHandle);
        }

        private async Task RunTests(KarmaSourceSettings settings, ITestLogger logger, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            logger = new TestLogger(logger, settings.Name, "Run");
            if (settings.Port > 0)
            {
                logger.Info("Start");
                var discoverCommand = new KarmaDiscoverCommand(settings.Port);
                await discoverCommand.Run(spec => RunTest(settings, logger, runContext, frameworkHandle, spec));
                logger.Info("Complete");
            }
            else
            {
                logger.Error("Not connected to karma");
            }
        }

        private void RunTest(KarmaSourceSettings settings, ITestLogger logger, IRunContext runContext, IFrameworkHandle frameworkHandle, KarmaSpec spec)
        {
            var testCase = KarmaTestDiscoverer.CreateTestCase(settings, spec);
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
                    DisplayName = result.Browser,
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

        private static TestOutcome GetTestOutcome(KarmaSpecResult result)
        {
            if (result.Skipped)
            {
                return TestOutcome.Skipped;
            }
            return result.Success ? TestOutcome.Passed : TestOutcome.Failed;
        }
    }
}