using KarmaTestAdapter.TestResults;
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
                [Fact(DisplayName = "GetAllChildren<Test>() should not return null")]
                public void GetAllChildrenShouldReturnNull()
                {
                    Assert.NotNull(Item.GetAllChildren<Test>());
                }

                [Fact(DisplayName = "GetAllChildren<Test>() should return empty list")]
                public void GetAllChildrenShouldReturnEmpty()
                {
                    Assert.Empty(Item.GetAllChildren<Test>());
                }
            }

            partial class WithChildren
            {
                [Fact(DisplayName = "GetAllChildren<Test>() should not return null")]
                public void GetAllChildrenShouldReturnNull()
                {
                    Assert.NotNull(Item.GetAllChildren<Test>());
                }

                [Fact(DisplayName = "GetAllChildren<Test>() should return non empty list")]
                public void GetAllChildrenShouldReturnNotEmpty()
                {
                    Assert.NotEmpty(Item.GetAllChildren<Test>());
                }

                [Fact(DisplayName = "GetAllChildren<Test>() should return list with 2 items")]
                public void GetAllChildrenShouldReturnTwoItems()
                {
                    Assert.Equal(2, Item.GetAllChildren<Test>().Count());
                }
            }
        }
    }
}
