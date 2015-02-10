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
                [Fact(DisplayName = "Name should not be null")]
                public void NameShouldNotBeNull()
                {
                    Assert.NotNull(Item.Name);
                }

                [Fact(DisplayName = "Name should be empty")]
                public void NameShouldBeEmpty()
                {
                    Assert.Equal("", Item.Name);
                }
            }
        }
    }
}
