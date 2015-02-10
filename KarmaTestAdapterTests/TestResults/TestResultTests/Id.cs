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
                [Fact(DisplayName = "Id should be null")]
                public void IdShouldBeNull()
                {
                    Assert.Null(Item.Id);
                }
            }

            [Fact(DisplayName = "Id should not be null")]
            public void IdShouldNotBeNull()
            {
                Assert.NotNull(Item.Id);
            }

            [Fact(DisplayName = "Id should be correct")]
            public void IdShouldBeCorrect()
            {
                Assert.Equal(2, Item.Id);
            }
        }
    }
}
