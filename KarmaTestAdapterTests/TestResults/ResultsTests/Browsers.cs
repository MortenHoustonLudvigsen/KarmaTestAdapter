using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.ResultsTests
{
    partial class TestResults
    {
        partial class Results
        {
            partial class Empty
            {
                [Fact(DisplayName = "Browsers should not be null")]
                public void BrowsersShouldNotBeNull()
                {
                    Assert.NotNull(Item.Browsers);
                }

                [Fact(DisplayName = "Browsers should be empty")]
                public void BrowsersShouldBeEmpty()
                {
                    Assert.Empty(Item.Browsers);
                }
            }

            [Fact(DisplayName = "Browsers should not be null")]
            public void BrowsersShouldNotBeNull()
            {
                Assert.NotNull(Item.Browsers);
            }

            [Fact(DisplayName = "Browsers should not be empty")]
            public void BrowsersShouldNotBeEmpty()
            {
                Assert.NotEmpty(Item.Browsers);
            }

            [Fact(DisplayName = "Browsers should have 3 items")]
            public void BrowsersShouldHaveTwoItems()
            {
                Assert.Equal(3, Item.Browsers.Count());
            }
        }
    }
}
