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

            [Fact(DisplayName = "Tests should not be empty")]
            public void TestsShouldBeEmpty()
            {
                Assert.NotEmpty(Item.Tests);
            }

            [Fact(DisplayName = "Tests should have 1 item")]
            public void TestsShouldHaveOneItem()
            {
                Assert.Equal(1, Item.Tests.Count());
            }
        }
    }
}
