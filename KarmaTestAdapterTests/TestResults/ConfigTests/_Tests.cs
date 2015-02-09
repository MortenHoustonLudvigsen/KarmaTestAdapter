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
        public class ConfigTestsHelper : Helper<KarmaTestAdapter.TestResults.Config>
        {
            public override KarmaTestAdapter.TestResults.Config CreateItem()
            {
                return CreateKarma().KarmaConfig;
            }
        }

        public class EmptyConfigTestsHelper : ConfigTestsHelper
        {
            public override KarmaTestAdapter.TestResults.Config CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Config(CreateKarma(), null);
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
