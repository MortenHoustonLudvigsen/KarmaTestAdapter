using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Karma;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            var karmaLogger = new KarmaLogger(logger, "Discover");
            karmaLogger.Debug("Start");
            karmaLogger.Debug("discoveryContext.RunSettings.SettingsXml: {0}", discoveryContext.RunSettings.SettingsXml);
            var testSettings = discoveryContext.RunSettings.GetKarmaTestSettings();
            if (testSettings == null)
            {
                karmaLogger.Error("Could not get karma settings");
            }
            else
            {
                foreach (var source in sources)
                {
                    var sourceSettings = testSettings.GetSource(source);
                    if (sourceSettings != null)
                    {
                        DiscoverTests(sourceSettings, karmaLogger, discoverySink).Wait();
                    }
                    else
                    {
                        karmaLogger.Debug("Could not get karma settings for {0}", source);
                    }
                }
            }
            karmaLogger.Debug("Finished");
        }

        private async Task DiscoverTests(KarmaSourceSettings settings, IKarmaLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            var tests = new ConcurrentBag<Guid>();
            logger.Info("Discovering tests for {0}", PathUtils.GetRelativePath(settings.BaseDirectory, settings.Source));
            if (settings.Port > 0)
            {
                var discoverCommand = new KarmaDiscoverCommand(settings.Port);
                discoverCommand.Connected += () => logger.Debug("Connected to karma");
                discoverCommand.Disconnected += () => logger.Debug("Disconnected from karma");
                await discoverCommand.Run(spec =>
                {
                    var testCase = CreateTestCase(settings, spec);
                    tests.Add(testCase.Id);
                    discoverySink.SendTestCase(testCase);
                });
                await new KarmaRequestRunCommand(settings.Port).Run(tests);
            }
            else
            {
                logger.Error("Not connected to karma");
            }
        }

        public static TestCase CreateTestCase(KarmaSourceSettings settings, KarmaSpec spec)
        {
            var testCase = new TestCase(spec.UniqueName, Globals.ExecutorUri, settings.Source);
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