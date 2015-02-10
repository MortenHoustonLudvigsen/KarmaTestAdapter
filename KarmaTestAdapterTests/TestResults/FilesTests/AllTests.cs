using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.FilesTests
{
    partial class TestResults
    {
        partial class Files
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

            [Fact(DisplayName = "AllTests should have 2 items")]
            public void AllTestsShouldHaveTwoItems()
            {
                Assert.Equal(2, Item.AllTests.Count());
            }
        }
    }
}
