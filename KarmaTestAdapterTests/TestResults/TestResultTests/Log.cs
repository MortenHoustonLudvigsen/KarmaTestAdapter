using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.TestResultTests
{
    partial class TestResults
    {
        partial class TestResult
        {
            partial class Empty
            {
                [Fact(DisplayName = "Log should not be null")]
                public void LogShouldNotBeNull()
                {
                    Assert.NotNull(Item.Log);
                }

                [Fact(DisplayName = "Log should be empty")]
                public void LogShouldBeEmpty()
                {
                    Assert.Empty(Item.Log);
                }
            }

            [Fact(DisplayName = "Log should not be null")]
            public void LogShouldNotBeNull()
            {
                Assert.NotNull(Item.Log);
            }

            [Fact(DisplayName = "Log should not be empty")]
            public void LogShouldNotBeEmpty()
            {
                Assert.NotEmpty(Item.Log);
            }

            [Fact(DisplayName = "Log should be correct")]
            public void LogShouldBeCorrect()
            {
                Assert.Equal(new[] { "Expected 15 to be 23." }, Item.Log);
            }
        }
    }
}
