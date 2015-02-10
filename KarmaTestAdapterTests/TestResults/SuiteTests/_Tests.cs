using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.SuiteTests
{
    public partial class TestResults
    {
        public class SuiteTestsHelper : Helper<KarmaTestAdapter.TestResults.Suite, KarmaTestAdapter.TestResults.TestCaseContainer>
        {
            public override KarmaTestAdapter.TestResults.TestCaseContainer CreateParent()
            {
                return CreateKarma().Files.Skip(2).First().Suites.First();
            }

            public override KarmaTestAdapter.TestResults.Suite CreateItem()
            {
                return CreateParent().Suites.First();
            }
        }

        public class EmptySuiteTestsHelper : SuiteTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Suite CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Suite(CreateParent(), null);
            }
        }

        public partial class Suite : SuiteTestsHelper.Tests<SuiteTestsHelper>
        {
            public partial class Empty : SuiteTestsHelper.Tests<EmptySuiteTestsHelper>
            {
            }
        }
    }
}
