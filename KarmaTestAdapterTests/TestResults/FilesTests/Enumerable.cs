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
                [Fact(DisplayName = "should not be null")]
                public void ShouldNotBeNull()
                {
                    Assert.NotNull(Item);
                }

                [Fact(DisplayName = "should be empty")]
                public void ShouldBeEmpty()
                {
                    Assert.Empty(Item);
                }
            }

            [Fact(DisplayName = "should not be null")]
            public void ShouldNotBeNull()
            {
                Assert.NotNull(Item);
            }

            [Fact(DisplayName = "should be non empty")]
            public void ShouldBeNonEmpty()
            {
                Assert.NotEmpty(Item);
            }

            [Fact(DisplayName = "should have 4 items")]
            public void ShouldHaveFourItems()
            {
                Assert.Equal(4, Item.Count());
            }
        }
    }
}
