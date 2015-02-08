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
            partial class WithoutParents
            {
                [Fact(DisplayName = @"GetParent<Item>() should return null")]
                public void GetParent_Item_ShouldReturnNull()
                {
                    Assert.Null(Item.GetParent<KarmaTestAdapter.TestResults.Item>());
                }
            }

            partial class WithParents
            {
                [Fact(DisplayName = @"GetParent<Karma>() should not return null")]
                public void GetParent_Karma_ShouldNotReturnNull()
                {
                    Assert.NotNull(Item.GetParent<Karma>());
                }

                [Fact(DisplayName = @"GetParent<Karma>() should return root")]
                public void GetParent_Karma_ShouldReturnRoot()
                {
                    Assert.Equal(Item.Root, Item.GetParent<Karma>());
                }

                [Fact(DisplayName = @"GetParent<Karma>() should return a Karma item")]
                public void ShouldBeKarma()
                {
                    Assert.IsType<Karma>(Item.GetParent<Karma>());
                }
            }
        }
    }
}
