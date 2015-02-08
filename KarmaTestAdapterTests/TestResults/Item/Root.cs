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
                [Fact(DisplayName = "Root should be null")]
                public void ShouldBeNull()
                {
                    Assert.Null(Item.Root);
                }
            }

            partial class WithoutParents
            {
                [Fact(DisplayName = @"Root should be null")]
                public void RootBeNull()
                {
                    Assert.Null(Item.Root);
                }
            }
            partial class WithParents
            {
                [Fact(DisplayName = @"Root should not be null")]
                public void RootShouldNotBeNull()
                {
                    Assert.NotNull(Item.Root);
                }

                [Fact(DisplayName = @"Root should be a Karma item")]
                public void RootShouldBeKarma()
                {
                    Assert.IsType<Karma>(Item.Root);
                }

                [Fact(DisplayName = @"Root.Parent should be null")]
                public void RootParentShouldBeNull()
                {
                    Assert.Null(Item.Root.Parent);
                }
            }
        }
    }
}
