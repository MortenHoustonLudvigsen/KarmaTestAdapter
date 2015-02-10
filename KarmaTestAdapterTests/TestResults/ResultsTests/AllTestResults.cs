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
                [Fact(DisplayName = "AllTestResults should not be null")]
                public void AllTestResultsShouldNotBeNull()
                {
                    Assert.NotNull(Item.AllTestResults);
                }

                [Fact(DisplayName = "AllTestResults should be empty")]
                public void AllTestResultsShouldBeEmpty()
                {
                    Assert.Empty(Item.AllTestResults);
                }
            }

            [Fact(DisplayName = "AllTestResults should not be null")]
            public void AllTestResultsShouldNotBeNull()
            {
                Assert.NotNull(Item.AllTestResults);
            }

            [Fact(DisplayName = "AllTestResults should not be empty")]
            public void AllTestResultsShouldNotBeEmpty()
            {
                Assert.NotEmpty(Item.AllTestResults);
            }

            [Fact(DisplayName = "AllTestResults should have 6 items")]
            public void AllTestResultsShouldHaveTwoItems()
            {
                Assert.Equal(6, Item.AllTestResults.Count());
            }
        }
    }
}
