using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.KarmaTests
{
    partial class TestResults
    {
        partial class Karma
        {
            partial class Empty
            {
                [Fact(DisplayName = "Results should not be null")]
                public void ResultsShouldNotBeNull()
                {
                    Assert.NotNull(Item.Results);
                }
            }

            [Fact(DisplayName = "Results should not be null")]
            public void ResultsShouldNotBeNull()
            {
                Assert.NotNull(Item.Results);
            }
        }
    }
}
