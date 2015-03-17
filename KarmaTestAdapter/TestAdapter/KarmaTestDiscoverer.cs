using JsTestAdapter.Logging;
using JsTestAdapter.TestAdapter.TestSettings;
using JsTestAdapter.TestServer;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Karma;
using KarmaTestAdapter.Logging;
using KarmaTestAdapter.TestAdapter.TestSettings;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KarmaTestAdapter.TestAdapter
{
    [FileExtension(".js")]
    [FileExtension(".json")]
    [DefaultExecutorUri(Globals.ExecutorUriString)]
    public class KarmaTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            var karmaLogger = new KarmaLogger(logger);
            var discoverLogger = new TestLogger(karmaLogger, "Discover");
            var testSettings = discoveryContext.RunSettings.GetKarmaTestSettings();
            foreach (var source in sources)
            {
                var sourceSettings = GetSourceSettings(source, testSettings);
                if (sourceSettings != null)
                {
                    DiscoverTests(sourceSettings, karmaLogger, discoverySink).Wait();
                }
                else
                {
                    discoverLogger.Warn("Could not get karma settings for {0}", source);
                }
            }
        }

        public static SourceSettings GetSourceSettings(string source, KarmaTestSettings testSettings)
        {
            SourceSettings sourceSettings = null;
            if (testSettings != null)
            {
                sourceSettings = testSettings.GetSource(source);
            }
            if (sourceSettings == null)
            {
                sourceSettings = SourceSettingsPersister.Load(Globals.GlobalLogDir, source);
            }
            return sourceSettings;
        }

        private async Task DiscoverTests(SourceSettings settings, ITestLogger logger, ITestCaseDiscoverySink discoverySink)
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
                logger.Error("Not connected to karma");
            }
        }

        public static TestCase CreateTestCase(SourceSettings settings, Spec spec)
        {
            var fullyQualifiedName = string.Format("{0} / {1}", settings.Name, spec.UniqueName);
            var testCase = new TestCase(fullyQualifiedName, Globals.ExecutorUri, settings.Source);
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
    }
}