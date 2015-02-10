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
                [Fact(DisplayName = "Message should not be null")]
                public void MessageShouldNotBeNull()
                {
                    Assert.NotNull(Item.Message);
                }

                [Fact(DisplayName = "Message should be correct")]
                public void MessageShouldBeCorrect()
                {
                    Assert.Equal("", Item.Message);
                }
            }

            [Fact(DisplayName = "Message should not be null")]
            public void MessageShouldNotBeNull()
            {
                Assert.NotNull(Item.Message);
            }

            [Fact(DisplayName = "Message should be correct")]
            public void MessageShouldBeCorrect()
            {
                Assert.Equal("Expected 15 to be 23.", Item.Message);
            }
        }
    }
}
