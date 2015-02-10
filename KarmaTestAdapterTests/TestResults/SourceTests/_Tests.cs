using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.SourceTests
{
    public partial class TestResults
    {
        public class SourceTestsHelper : Helper<KarmaTestAdapter.TestResults.Source, KarmaTestAdapter.TestResults.TestCaseContainer>
        {
            public override KarmaTestAdapter.TestResults.TestCaseContainer CreateParent()
            {
                return CreateKarma().Files.Skip(2).First().Suites.First();
            }

            public override KarmaTestAdapter.TestResults.Source CreateItem()
            {
                return CreateParent().Source;
            }
        }

        public class EmptySourceTestsHelper : SourceTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Source CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Source(CreateParent(), null);
            }
        }

        public partial class Source : SourceTestsHelper.Tests<SourceTestsHelper>
        {
            public partial class Empty : SourceTestsHelper.Tests<EmptySourceTestsHelper>
            {
            }
        }
    }
}
