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
                [Fact(DisplayName = "TestResults should not be null")]
                public void TestResultsShouldNotBeNull()
                {
                    Assert.NotNull(Item.TestResults);
                }

                [Fact(DisplayName = "TestResults should be empty")]
                public void TestResultsShouldBeEmpty()
                {
                    Assert.Empty(Item.TestResults);
                }
            }

            [Fact(DisplayName = "TestResults should not be null")]
            public void TestResultsShouldNotBeNull()
            {
                Assert.NotNull(Item.TestResults);
            }

            [Fact(DisplayName = "TestResults should be empty")]
            public void TestResultsShouldBeEmpty()
            {
                Assert.Empty(Item.TestResults);
            }
        }
    }
}
