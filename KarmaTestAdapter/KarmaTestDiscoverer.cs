using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using TwoPS.Processes;
using IO = System.IO;

namespace KarmaTestAdapter
{
    [FileExtension(Globals.JavaScriptExtension)]
    [FileExtension(Globals.JsonExtension)]
    [DefaultExecutorUri(Globals.ExecutorUriString)]
    public class KarmaTestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            var karmaLogger = KarmaLogger.Create(messageLogger: logger);
            karmaLogger.Info("DiscoverTests start");
            foreach (var testcase in GetTests(sources, discoveryContext, karmaLogger))
            {
                discoverySink.SendTestCase(testcase);
            }
            karmaLogger.Info("DiscoverTests end");
        }

        public static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IKarmaLogger logger)
        {
            try
            {
                return sources.SelectMany(s => GetTests(s, logger)).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return Enumerable.Empty<TestCase>();
            }
        }

        public static IEnumerable<TestCase> GetTests(string source, IKarmaLogger logger)
        {
            logger.Info("Source: {0}", source);
            var karma = KarmaReporter.Discover(source, logger);
            return karma.Files.AllTests.Select(test => CreateTestCase(test, source));
        }

        public static TestCase CreateTestCase(Test test, string source)
        {
            var testCase = new TestCase(test.FullyQualifiedName, Globals.ExecutorUri, source);
            testCase.DisplayName = test.DisplayName;
            if (test.Source != null)
            {
                testCase.CodeFilePath = test.Source.FullPath;
                if (test.Source.Line.HasValue)
                {
                    testCase.LineNumber = test.Source.Line.Value;
                }
            }
            else
            {
                testCase.CodeFilePath = test.File.FullPath;
                if (test.Line.HasValue)
                {
                    testCase.LineNumber = test.Line.Value;
                }
            }
            return testCase;
        }
    }
}
