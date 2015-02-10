using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.KarmaTests
{
    public partial class TestResults
    {
        public class KarmaTestsHelper : Helper<KarmaTestAdapter.TestResults.Karma, KarmaTestAdapter.TestResults.Item>
        {
            public override KarmaTestAdapter.TestResults.Item CreateParent()
            {
                return null;
            }

            public override KarmaTestAdapter.TestResults.Karma CreateItem()
            {
                return CreateKarma();
            }
        }

        public class EmptyKarmaTestsHelper : KarmaTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Karma CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Karma((XElement)null);
            }
        }

        public partial class Karma : KarmaTestsHelper.Tests<KarmaTestsHelper>
        {
            public partial class Empty : KarmaTestsHelper.Tests<EmptyKarmaTestsHelper>
            {
            }
        }
    }
}
