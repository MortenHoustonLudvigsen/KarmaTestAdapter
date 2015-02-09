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
                [Fact(DisplayName = "Served should be null")]
                public void ServedShouldBeNull()
                {
                    Assert.Null(Item.Served);
                }
            }

            [Fact(DisplayName = "Served should not be null")]
            public void ServedShouldNotBeNull()
            {
                Assert.NotNull(Item.Served);
            }

            [Fact(DisplayName = "Served should be correct")]
            public void ServedShouldBeCorrect()
            {
                Assert.Equal(true, Item.Served);
            }
        }
    }
}
