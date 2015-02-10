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
                [Fact(DisplayName = "Tests should not be null")]
                public void TestsShouldNotBeNull()
                {
                    Assert.NotNull(Item.Tests);
                }

                [Fact(DisplayName = "Tests should be empty")]
                public void TestsShouldBeEmpty()
                {
                    Assert.Empty(Item.Tests);
                }
            }

            [Fact(DisplayName = "Tests should not be null")]
            public void TestsShouldNotBeNull()
            {
                Assert.NotNull(Item.Tests);
            }

            [Fact(DisplayName = "Tests should be empty")]
            public void TestsShouldBeEmpty()
            {
                Assert.Empty(Item.Tests);
            }
        }
    }
}
