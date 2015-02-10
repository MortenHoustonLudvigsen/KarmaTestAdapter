using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.SuiteResultTests
{
    partial class TestResults
    {
        partial class SuiteResult
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

            [Fact(DisplayName = "AllTestResults should have 1 item")]
            public void AllTestResultsShouldHaveOneItem()
            {
                Assert.Equal(1, Item.AllTestResults.Count());
            }
        }
    }
}
