using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.ConfigTests
{
    partial class TestResults
    {
        partial class Config
        {
            partial class Empty
            {
                [Fact(DisplayName = "BasePath should be null")]
                public void BasePathShouldBeNull()
                {
                    Assert.Null(Item.BasePath);
                }
            }

            [Fact(DisplayName = "BasePath should not be null")]
            public void BasePathShouldNotBeNull()
            {
                Assert.NotNull(Item.BasePath);
            }

            [Fact(DisplayName = "BasePath should be correct")]
            public void BasePathShouldBeCorrect()
            {
                Assert.Equal(@"C:/Git/KarmaTestAdapter/karma-vs-reporter/test", Item.BasePath);
            }
        }
    }
}
