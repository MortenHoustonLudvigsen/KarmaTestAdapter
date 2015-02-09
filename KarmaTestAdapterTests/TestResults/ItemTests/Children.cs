using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.ItemTests
{
    partial class TestResults
    {
        partial class Item
        {
            partial class Empty
            {
                [Fact(DisplayName = "Children should not be null")]
                public void ChildrenShouldNotBeNull()
                {
                    Assert.NotNull(Item.Children);
                }

                [Fact(DisplayName = "Children should be empty")]
                public void ChildrenShouldBeEmpty()
                {
                    Assert.Empty(Item.Children);
                }
            }

            partial class WithChildren
            {
                [Fact(DisplayName = "Children should not be null")]
                public void ChildrenShouldNotBeNull()
                {
                    Assert.NotNull(Item.Children);
                }

                [Fact(DisplayName = "Children should not be empty")]
                public void ChildrenShouldBeEmpty()
                {
                    Assert.NotEmpty(Item.Children);
                }
            }
        }
    }
}
