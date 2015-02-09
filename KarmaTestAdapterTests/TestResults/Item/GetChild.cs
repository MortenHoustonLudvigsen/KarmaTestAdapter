using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.Item
{
    partial class TestResults
    {
        partial class Item
        {
            partial class Empty
            {
                [Fact(DisplayName = "GetChild<Config>() should return null")]
                public void GetChildShouldReturnNull()
                {
                    Assert.Null(Item.GetChild<Config>());
                }
            }

            partial class WithChildren
            {
                [Fact(DisplayName = "GetChild<Config>() should not return null")]
                public void GetChildShouldNotReturnNull()
                {
                    Assert.NotNull(Item.GetChild<Config>());
                }

                [Fact(DisplayName = "GetChild<Config>() should return item of type Config")]
                public void GetChildShouldReturnConfig()
                {
                    Assert.IsType<Config>(Item.GetChild<Config>());
                }
            }
        }
    }
}
