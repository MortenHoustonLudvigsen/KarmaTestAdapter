using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.ItemTests
{
    partial class TestResults
    {
        partial class Item
        {
            partial class Empty
            {
                [Fact(DisplayName = "should not be null")]
                public void ShouldNotBeNull()
                {
                    Assert.NotNull(Item);
                }
            }
        }
    }
}
