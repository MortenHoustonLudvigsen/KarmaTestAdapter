using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IO = System.IO;

namespace KarmaTestAdapter.TestResults
{
    public class Karma : Item
    {
        public static Karma Load(string filename)
        {
            return new Karma(XDocument.Load(filename));
        }

        public Karma(XDocument document)
            : this(document.Element("Karma"))
        {
        }

        public Karma(XElement element)
            : base(null, element)
        {
            KarmaConfig = GetChild<Config>() ?? new Config(this, null);
            Files = GetChild<Files>() ?? new Files(this, null);
            Results = GetChild<Results>() ?? new Results(this, null);
        }

        public Config KarmaConfig { get; private set; }
        public Files Files { get; private set; }
        public Results Results { get; private set; }

        public IEnumerable<TestCase> GetTestCases(string source)
        {
            return Files.AllTests.Select(test => CreateTestCase(test, source));
        }

        public static TestCase CreateTestCase(Test test, string source)
        {
            var testCase = new TestCase(test.FullyQualifiedName, Globals.ExecutorUri, source);
            testCase.DisplayName = test.DisplayName;
            testCase.SetPropertyValue(Globals.FileIndexProperty, test.Index);
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

        public IEnumerable<ConsolidatedTestResult> ConsolidateResults()
        {
            var tests = Files.AllTests.Select(t => new ConsolidatedTestResult(t)).ToList();

            if (Results != null)
            {
                foreach (var browser in Results.Browsers)
                {
                    foreach (var result in browser.AllTestResults)
                    {
                        var test = tests.FirstOrDefault(t => t.Test.DisplayName == result.DisplayName);
                        if (test != null)
                        {
                            test.Results.Add(result);
                        }
                    }
                }
            }

            return tests;
        }
    }
}
