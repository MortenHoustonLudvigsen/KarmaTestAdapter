using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests.TestResults
{
    public abstract class Helper<TItem>
        where TItem : KarmaTestAdapter.TestResults.Item
    {
        public abstract TItem CreateItem();

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