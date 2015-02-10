using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults.ConfigTests
{
    public partial class TestResults
    {
        public class ConfigTestsHelper : Helper<KarmaTestAdapter.TestResults.Config, KarmaTestAdapter.TestResults.Karma>
        {
            public override KarmaTestAdapter.TestResults.Karma CreateParent()
            {
                return CreateKarma();
            }

            public override KarmaTestAdapter.TestResults.Config CreateItem()
            {
                return CreateParent().KarmaConfig;
            }
        }

        public class EmptyConfigTestsHelper : ConfigTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Config CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Config(CreateParent(), null);
            }
        }

        public partial class Config : ConfigTestsHelper.Tests<ConfigTestsHelper>
        {
            public partial class Empty : ConfigTestsHelper.Tests<EmptyConfigTestsHelper>
            {
            }
        }
    }
}
