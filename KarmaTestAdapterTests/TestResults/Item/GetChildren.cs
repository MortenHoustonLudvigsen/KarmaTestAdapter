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
                [Fact(DisplayName = "GetChildren<Config>() should not return null")]
                public void GetChildrenShouldReturnNull()
                {
                    Assert.NotNull(Item.GetChildren<Config>());
                }

                [Fact(DisplayName = "GetChildren<Config>() should return empty list")]
                public void GetChildrenShouldReturnEmpty()
                {
                    Assert.Empty(Item.GetChildren<Config>());
                }
            }

            partial class WithChildren
            {
                [Fact(DisplayName = "GetChildren<Config>() should not return null")]
                public void GetChildrenShouldReturnNull()
                {
                    Assert.NotNull(Item.GetChildren<Config>());
                }

                [Fact(DisplayName = "GetChildren<Config>() should return non empty list")]
                public void GetChildrenShouldReturnNotEmpty()
                {
                    Assert.NotEmpty(Item.GetChildren<Config>());
                }

                [Fact(DisplayName = "GetChildren<Config>() should return list with 1 item")]
                public void GetChildrenShouldReturnOneItem()
                {
                    Assert.Equal(1, Item.GetChildren<Config>().Count());
                }
            }
        }
    }
}
