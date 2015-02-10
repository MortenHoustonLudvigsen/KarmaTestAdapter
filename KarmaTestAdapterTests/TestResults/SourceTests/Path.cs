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
                [Fact(DisplayName = "Path should be null")]
                public void PathShouldBeNull()
                {
                    Assert.Null(Item.Path);
                }
            }

            [Fact(DisplayName = "Path should not be null")]
            public void PathShouldNotBeNull()
            {
                Assert.NotNull(Item.Path);
            }

            [Fact(DisplayName = "Path should be correct")]
            public void PathShouldBeCorrect()
            {
                Assert.Equal(@"C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/TestFile2.ts", Item.Path);
            }
        }
    }
}
