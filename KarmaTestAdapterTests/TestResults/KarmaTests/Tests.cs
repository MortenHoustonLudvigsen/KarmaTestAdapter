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
        public abstract class KarmaTestsHelper : Helper<KarmaTestAdapter.TestResults.Karma>
        {
            public virtual XElement GetElement()
            {
                return XDocument.Parse(Constants.KarmaXml).Root;
            }

            public override KarmaTestAdapter.TestResults.Karma CreateItem()
            {
                return new KarmaTestAdapter.TestResults.Karma(GetElement());
            }
        }

        public partial class Karma : KarmaTestsHelper.Tests<Karma.Helper>
        {
            public class Helper : KarmaTestsHelper
            {
            }

            public partial class Empty : KarmaTestsHelper.Tests<Empty.Helper>
            {
                public class Helper : KarmaTestsHelper
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
