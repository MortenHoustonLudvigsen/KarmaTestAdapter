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
                [Fact(DisplayName = "ConsolidateResults() should return not null")]
                public void ConsolidateResultsShouldReturnNotNull()
                {
                    Assert.NotNull(Item.ConsolidateResults());
                }

                [Fact(DisplayName = "ConsolidateResults() should return empty list")]
                public void ConsolidateResultsShouldReturnEmpty()
                {
                    Assert.Empty(Item.ConsolidateResults());
                }
            }

            [Fact(DisplayName = "ConsolidateResults() should return not null")]
            public void ConsolidateResultsShouldReturnNotNull()
            {
                Assert.NotNull(Item.ConsolidateResults());
            }

            [Fact(DisplayName = "ConsolidateResults() should return non empty list")]
            public void ConsolidateResultsShouldReturnNonEmpty()
            {
                Assert.NotEmpty(Item.ConsolidateResults());
            }

            [Fact(DisplayName = "ConsolidateResults() should return two items")]
            public void ConsolidateResultsShouldReturnTwoItems()
            {
                Assert.Equal(2, Item.ConsolidateResults().Count());
            }
        }
    }
}
