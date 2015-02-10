using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.TestResultTests
{
    public partial class TestResults
    {
        public class TestResultTestsHelper : Helper<KarmaTestAdapter.TestResults.TestResult, KarmaTestAdapter.TestResults.ResultContainer>
        {
            public override KarmaTestAdapter.TestResults.ResultContainer CreateParent()
            {
                return CreateKarma().Results.Browsers.First().Suites.First().Suites.First();
            }

            public override KarmaTestAdapter.TestResults.TestResult CreateItem()
            {
                return CreateParent().AllTestResults.First();
            }
        }

        public class EmptyTestResultTestsHelper : TestResultTestsHelper
        {
            public override KarmaTestAdapter.TestResults.TestResult CreateItem()
            {
                return new KarmaTestAdapter.TestResults.TestResult(CreateParent(), null);
            }
        }

        public partial class TestResult : TestResultTestsHelper.Tests<TestResultTestsHelper>
        {
            public partial class Empty : TestResultTestsHelper.Tests<EmptyTestResultTestsHelper>
            {
            }
        }
    }
}
