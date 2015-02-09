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
        public abstract class ConfigTestsHelper : Helper<KarmaTestAdapter.TestResults.Config>
        {
            public virtual XElement GetElement()
            {
                return XDocument.Parse(Constants.KarmaXml).Root.Element("Config");
            }

            public override KarmaTestAdapter.TestResults.Config CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Config(null, GetElement());
            }
        }

        public partial class Config : ConfigTestsHelper.Tests<Config.Helper>
        {
            public class Helper : ConfigTestsHelper
            {
            }

            public partial class Empty : ConfigTestsHelper.Tests<Empty.Helper>
            {
                public class Helper : ConfigTestsHelper
                {
                    public override XElement GetElement()
                    {
                        return null;
                    }
                }
            }
        }
    }
}
