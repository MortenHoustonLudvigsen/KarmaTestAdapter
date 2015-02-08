using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.Item
{
    partial class TestResults
    {
        partial class Item
        {
            [Fact(DisplayName = "Create(<Karma..>) should return a Karma item")]
            public void ReturnsKarmaItemGivenKarmaElement()
            {
                Assert.IsType<Karma>(Item.Create(new XElement("Karma")));
            }

            [Fact(DisplayName = "Create(<Config..>) should return a Config item")]
            public void ReturnsConfigItemGivenConfigElement()
            {
                Assert.IsType<Config>(Item.Create(new XElement("Config")));
            }

            [Fact(DisplayName = "Create(<Files..>) should return a Files item")]
            public void ReturnsFilesItemGivenFilesElement()
            {
                Assert.IsType<Files>(Item.Create(new XElement("Files")));
            }

            [Fact(DisplayName = "Create(<File..>) should return a File item")]
            public void ReturnsFileItemGivenFileElement()
            {
                Assert.IsType<File>(Item.Create(new XElement("File")));
            }

            [Fact(DisplayName = "Create(<Suite..>) should return a Suite item")]
            public void ReturnsSuiteItemGivenSuiteElement()
            {
                Assert.IsType<Suite>(Item.Create(new XElement("Suite")));
            }

            [Fact(DisplayName = "Create(<Test..>) should return a Test item")]
            public void ReturnsTestItemGivenTestElement()
            {
                Assert.IsType<Test>(Item.Create(new XElement("Test")));
            }

            [Fact(DisplayName = "Create(<Source..>) should return a Source item")]
            public void ReturnsSourceItemGivenSourceElement()
            {
                Assert.IsType<Source>(Item.Create(new XElement("Source")));
            }

            [Fact(DisplayName = "Create(<Results..>) should return a Results item")]
            public void ReturnsResultsItemGivenResultsElement()
            {
                Assert.IsType<Results>(Item.Create(new XElement("Results")));
            }

            [Fact(DisplayName = "Create(<Browser..>) should return a Browser item")]
            public void ReturnsBrowserItemGivenBrowserElement()
            {
                Assert.IsType<Browser>(Item.Create(new XElement("Browser")));
            }

            [Fact(DisplayName = "Create(<SuiteResult..>) should return a SuiteResult item")]
            public void ReturnsSuiteResultItemGivenSuiteResultElement()
            {
                Assert.IsType<SuiteResult>(Item.Create(new XElement("SuiteResult")));
            }

            [Fact(DisplayName = "Create(<TestResult..>) should return a TestResult item")]
            public void ReturnsTestResultItemGivenTestResultElement()
            {
                Assert.IsType<TestResult>(Item.Create(new XElement("TestResult")));
            }

            [Fact(DisplayName = "Create(<XXX..>) should return an UnknownItem item")]
            public void ReturnsUnknownItemGivenUnknownElement()
            {
                Assert.IsType<UnknownItem>(Item.Create(new XElement("XXX")));
            }
        }
    }
}
