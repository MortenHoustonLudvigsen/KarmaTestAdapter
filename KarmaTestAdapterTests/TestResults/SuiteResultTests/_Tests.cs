using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.SuiteResultTests
{
    public partial class TestResults
    {
        public class SuiteResultTestsHelper : Helper<KarmaTestAdapter.TestResults.SuiteResult, KarmaTestAdapter.TestResults.ResultContainer>
        {
            public override KarmaTestAdapter.TestResults.ResultContainer CreateParent()
            {
                return CreateKarma().Results.Browsers.First().Suites.First();
            }

            public override KarmaTestAdapter.TestResults.SuiteResult CreateItem()
            {
                return CreateParent().Suites.First();
            }
        }

        public class EmptySuiteResultTestsHelper : SuiteResultTestsHelper
        {
            public override KarmaTestAdapter.TestResults.SuiteResult CreateItem()
            {
                return new KarmaTestAdapter.TestResults.SuiteResult(CreateParent(), null);
            }
        }

        public partial class SuiteResult : SuiteResultTestsHelper.Tests<SuiteResultTestsHelper>
        {
            public partial class Empty : SuiteResultTestsHelper.Tests<EmptySuiteResultTestsHelper>
            {
            }
        }
    }
}
