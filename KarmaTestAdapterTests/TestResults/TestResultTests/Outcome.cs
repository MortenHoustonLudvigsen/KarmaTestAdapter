using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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
                [Fact(DisplayName = "Outcome should not be null")]
                public void OutcomeShouldNotBeNull()
                {
                    Assert.NotNull(Item.Outcome);
                }

                [Fact(DisplayName = "Outcome should be none")]
                public void OutcomeShouldBeNone()
                {
                    Assert.Equal(TestOutcome.None, Item.Outcome);
                }
            }

            [Fact(DisplayName = "Outcome should not be null")]
            public void OutcomeShouldNotBeNull()
            {
                Assert.NotNull(Item.Outcome);
            }

            [Fact(DisplayName = "Outcome should be failed")]
            public void OutcomeShouldBeFailed()
            {
                Assert.Equal(TestOutcome.Failed, Item.Outcome);
            }
        }
    }
}
