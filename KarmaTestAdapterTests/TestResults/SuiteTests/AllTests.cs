using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.SuiteTests
{
    partial class TestResults
    {
        partial class Suite
        {
            partial class Empty
            {
                [Fact(DisplayName = "AllTests should not be null")]
                public void AllTestsShouldNotBeNull()
                {
                    Assert.NotNull(Item.AllTests);
                }

                [Fact(DisplayName = "AllTests should be empty")]
                public void AllTestsShouldBeEmpty()
                {
                    Assert.Empty(Item.AllTests);
                }
            }

            [Fact(DisplayName = "AllTests should not be null")]
            public void AllTestsShouldNotBeNull()
            {
                Assert.NotNull(Item.AllTests);
            }

            [Fact(DisplayName = "AllTests should not be empty")]
            public void AllTestsShouldNotBeEmpty()
            {
                Assert.NotEmpty(Item.AllTests);
            }

            [Fact(DisplayName = "AllTests should have 1 item")]
            public void AllTestsShouldHaveOneItem()
            {
                Assert.Equal(1, Item.AllTests.Count());
            }
        }
    }
}
