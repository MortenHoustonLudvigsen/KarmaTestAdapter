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
                [Fact(DisplayName = "Time should not be null")]
                public void TimeShouldNotBeNull()
                {
                    Assert.NotNull(Item.Time);
                }

                [Fact(DisplayName = "Time should be correct")]
                public void TimeShouldBeCorrect()
                {
                    Assert.Equal(TimeSpan.FromMilliseconds(0.5), Item.Time);
                }
            }

            [Fact(DisplayName = "Time should not be null")]
            public void TimeShouldNotBeNull()
            {
                Assert.NotNull(Item.Time);
            }

            [Fact(DisplayName = "Time should be correct")]
            public void TimeShouldBeCorrect()
            {
                Assert.Equal(TimeSpan.FromMilliseconds(5), Item.Time);
            }
        }
    }
}
