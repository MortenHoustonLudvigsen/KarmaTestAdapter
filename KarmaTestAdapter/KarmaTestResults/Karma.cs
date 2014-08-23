using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using IO = System.IO;

namespace KarmaTestAdapter.KarmaTestResults
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
        }

        public Config KarmaConfig { get { return Children.OfType<Config>().FirstOrDefault(); } }
        public Files Files { get { return Children.OfType<Files>().FirstOrDefault(); } }
        public Results Results { get { return Children.OfType<Results>().FirstOrDefault(); } }

        public IEnumerable<ConsolidatedTestResult> ConsolidateResults()
        {
            if (Files == null)
            {
                return Enumerable.Empty<ConsolidatedTestResult>();
            }
            var tests = Files.AllTests.Select(t => new ConsolidatedTestResult(t)).ToList();

            if (Results != null)
            {
                foreach (var browser in Results.Browsers)
                {
                    foreach (var result in browser.AllTestResults.Select((r, i) => new { TestResult = r, Index = i }))
                    {
                        if (result.Index >= tests.Count || result.TestResult.DisplayName != tests[result.Index].Test.DisplayName)
                        {
                            return Enumerable.Empty<ConsolidatedTestResult>();
                        }
                        tests[result.Index].Results.Add(result.TestResult);
                    }
                }
            }

            return tests;
        }
    }
}
