using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.FileTests
{
    partial class TestResults
    {
        partial class File
        {
            partial class Empty
            {
                [Fact(DisplayName = "Name should be null")]
                public void NameShouldBeNull()
                {
                    Assert.Null(Item.Name);
                }
            }

            [Fact(DisplayName = "Name should not be null")]
            public void NameShouldNotBeNull()
            {
                Assert.NotNull(Item.Name);
            }

            [Fact(DisplayName = "Name should be correct")]
            public void NameShouldBeCorrect()
            {
                Assert.Equal(@"C:/Git/KarmaTestAdapter/karma-vs-reporter/test/testfiles/TestFile2.js", Item.Name);
            }
        }
    }
}
