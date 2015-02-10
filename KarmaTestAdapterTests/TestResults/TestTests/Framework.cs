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
                [Fact(DisplayName = "Framework should be null")]
                public void FrameworkShouldBeNull()
                {
                    Assert.Null(Item.Framework);
                }
            }

            [Fact(DisplayName = "Framework should not be null")]
            public void FrameworkShouldNotBeNull()
            {
                Assert.NotNull(Item.Framework);
            }

            [Fact(DisplayName = "Framework should be correct")]
            public void FrameworkShouldBeCorrect()
            {
                Assert.Equal("jasmine", Item.Framework);
            }
        }
    }
}
