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
        }
    }
}
