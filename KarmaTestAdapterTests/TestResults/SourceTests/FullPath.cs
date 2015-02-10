using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.SourceTests
{
    partial class TestResults
    {
        partial class Source
        {
            partial class Empty
            {
                [Fact(DisplayName = "FullPath should be null")]
                public void FullPathShouldBeNull()
                {
                    Assert.Null(Item.FullPath);
                }
            }

            [Fact(DisplayName = "FullPath should not be null")]
            public void FullPathShouldNotBeNull()
            {
                Assert.NotNull(Item.FullPath);
            }

            [Fact(DisplayName = "FullPath should be correct")]
            public void FullPathShouldBeCorrect()
            {
                Assert.Equal(@"C:\Git\KarmaTestAdapter\karma-vs-reporter\test\testfiles\TestFile2.ts", Item.FullPath);
            }
        }
    }
}
