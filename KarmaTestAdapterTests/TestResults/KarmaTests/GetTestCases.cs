using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.KarmaTests
{
    partial class TestResults
    {
        partial class Karma
        {
            partial class Empty
            {
                [Fact(DisplayName = "GetTestCases(<source>) should return not null")]
                public void GetTestCasesShouldReturnNotNull()
                {
                    Assert.NotNull(Item.GetTestCases("Source"));
                }

                [Fact(DisplayName = "GetTestCases(<source>) should return empty list")]
                public void GetTestCasesShouldReturnEmpty()
                {
                    Assert.Empty(Item.GetTestCases("Source"));
                }
            }

            [Fact(DisplayName = "GetTestCases(<source>) should return not null")]
            public void GetTestCasesShouldReturnNotNull()
            {
                Assert.NotNull(Item.GetTestCases("Source"));
            }

            [Fact(DisplayName = "GetTestCases(<source>) should return non empty list")]
            public void GetTestCasesShouldReturnNonEmpty()
            {
                Assert.NotEmpty(Item.GetTestCases("Source"));
            }

            [Fact(DisplayName = "GetTestCases(<source>) should return two items")]
            public void GetTestCasesShouldReturnTwoItems()
            {
                Assert.Equal(2, Item.GetTestCases("Source").Count());
            }
        }
    }
}
