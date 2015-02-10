using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.ResultsTests
{
    public partial class TestResults
    {
        public class ResultsTestsHelper : Helper<KarmaTestAdapter.TestResults.Results>
        {
            public override KarmaTestAdapter.TestResults.Results CreateItem()
            {
                return CreateKarma().Results;
            }
        }

        public class EmptyResultsTestsHelper : ResultsTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Results CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Results(CreateKarma(), null);
            }
        }

        public partial class Results : ResultsTestsHelper.Tests<ResultsTestsHelper>
        {
            public partial class Empty : ResultsTestsHelper.Tests<EmptyResultsTestsHelper>
            {
            }
        }
    }
}
