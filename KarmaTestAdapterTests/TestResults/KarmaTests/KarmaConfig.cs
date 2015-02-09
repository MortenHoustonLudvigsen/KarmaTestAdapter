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
                [Fact(DisplayName = "KarmaConfig should not be null")]
                public void KarmaConfigShouldNotBeNull()
                {
                    Assert.NotNull(Item.KarmaConfig);
                }
            }

            [Fact(DisplayName = "KarmaConfig should not be null")]
            public void KarmaConfigShouldNotBeNull()
            {
                Assert.NotNull(Item.KarmaConfig);
            }
        }
    }
}
