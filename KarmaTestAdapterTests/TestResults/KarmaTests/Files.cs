using KarmaTestAdapter.TestResults;
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
                [Fact(DisplayName = "Files should not be null")]
                public void FilesShouldNotBeNull()
                {
                    Assert.NotNull(Item.Files);
                }

                [Fact(DisplayName = "Files should be empty")]
                public void FilesShouldBeEmpty()
                {
                    Assert.Empty(Item.Files);
                }
            }

            [Fact(DisplayName = "Files should not be null")]
            public void FilesShouldNotBeNull()
            {
                Assert.NotNull(Item.Files);
            }

            [Fact(DisplayName = "Files should be non empty")]
            public void FilesShouldBeNonEmpty()
            {
                Assert.NotEmpty(Item.Files);
            }
        }
    }
}
