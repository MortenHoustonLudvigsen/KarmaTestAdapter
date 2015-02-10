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
        public class SourceTestsHelper : Helper<KarmaTestAdapter.TestResults.Source>
        {
            public override KarmaTestAdapter.TestResults.Source CreateItem()
            {
                return CreateKarma().Files.Skip(2).First().Suites.First().Source;
            }
        }

        public class EmptySourceTestsHelper : SourceTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Source CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Source(CreateKarma().Files.Skip(2).First().Suites.First(), null);
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
