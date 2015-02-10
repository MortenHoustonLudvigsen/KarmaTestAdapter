using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.TestTests
{
    partial class TestResults
    {
        partial class Test
        {
            partial class Empty
            {
                [Fact(DisplayName = "Source should be null")]
                public void SourceShouldBeNull()
                {
                    Assert.Null(Item.Source);
                }
            }

            [Fact(DisplayName = "Source should not be null")]
            public void SourceShouldNotBeNull()
            {
                Assert.NotNull(Item.Source);
            }
        }
    }
}
