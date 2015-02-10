using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.TestTests
{
    public partial class TestResults
    {
        public class TestTestsHelper : Helper<KarmaTestAdapter.TestResults.Test, KarmaTestAdapter.TestResults.TestCaseContainer>
        {
            public override KarmaTestAdapter.TestResults.TestCaseContainer CreateParent()
            {
                return CreateKarma().Files;
            }

            public override KarmaTestAdapter.TestResults.Test CreateItem()
            {
                return CreateParent().AllTests.First();
            }
        }

        public class EmptyTestTestsHelper : TestTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Test CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Test(CreateParent(), null);
            }
        }

        public partial class Test : TestTestsHelper.Tests<TestTestsHelper>
        {
            public partial class Empty : TestTestsHelper.Tests<EmptyTestTestsHelper>
            {
            }
        }
    }
}
