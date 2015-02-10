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
                [Fact(DisplayName = "ParentSuite should be null")]
                public void ParentSuiteShouldBeNull()
                {
                    Assert.Null(Item.ParentSuite);
                }
            }

            [Fact(DisplayName = "ParentSuite should not be null")]
            public void ParentSuiteShouldNotBeNull()
            {
                Assert.NotNull(Item.ParentSuite);
            }
        }
    }
}
