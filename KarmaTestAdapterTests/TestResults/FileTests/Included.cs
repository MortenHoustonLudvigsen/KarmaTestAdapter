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
                [Fact(DisplayName = "Included should be null")]
                public void IncludedShouldBeNull()
                {
                    Assert.Null(Item.Included);
                }
            }

            [Fact(DisplayName = "Included should not be null")]
            public void IncludedShouldNotBeNull()
            {
                Assert.NotNull(Item.Included);
            }

            [Fact(DisplayName = "Included should be correct")]
            public void IncludedShouldBeCorrect()
            {
                Assert.Equal(true, Item.Included);
            }
        }
    }
}
