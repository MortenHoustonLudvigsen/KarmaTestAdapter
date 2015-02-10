using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.TestResultTests
{
    partial class TestResults
    {
        partial class TestResult
        {
            partial class Empty
            {
                [Fact(DisplayName = "ParentSuite should not be null")]
                public void ParentSuiteShouldNotBeNull()
                {
                    Assert.NotNull(Item.ParentSuite);
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
