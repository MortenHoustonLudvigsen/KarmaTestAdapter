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
                [Fact(DisplayName = "File should be null")]
                public void FileShouldBeNull()
                {
                    Assert.Null(Item.File);
                }
            }

            [Fact(DisplayName = "File should not be null")]
            public void FileShouldNotBeNull()
            {
                Assert.NotNull(Item.File);
            }
        }
    }
}
