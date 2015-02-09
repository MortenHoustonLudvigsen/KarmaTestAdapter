using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests.TestResults
{
    public abstract class Helper<TItem>
        where TItem : KarmaTestAdapter.TestResults.Item
    {
        public abstract TItem CreateItem();

        public KarmaTestAdapter.TestResults.Karma CreateKarma()
        {
            return new KarmaTestAdapter.TestResults.Karma(XDocument.Parse(Constants.KarmaXml).Root);
        }

        public abstract class Tests<THelper>
            where THelper : Helper<TItem>, new()
        {
            protected THelper _helper = new THelper();
            protected TItem Item { get; set; }

            public Tests()
            {
                Init();
            }

            public virtual void Init()
            {
                Item = _helper.CreateItem();
            }
        }
    }
}