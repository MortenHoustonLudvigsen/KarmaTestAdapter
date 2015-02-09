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
                [Fact(DisplayName = @"ValueOfElement(""Slam"") should return null")]
                public void ValueOfElement_Slam_ShouldReturnNull()
                {
                    Assert.Null(Item.ValueOfElement("Slam"));
                }
            }

            partial class WithElement
            {
                [Fact(DisplayName = @"ValueOfElement(""Slam"") should return ""Bam""")]
                public void ValueOfElement_Slam_ShouldReturnBam()
                {
                    Assert.Equal("Bam", Item.ValueOfElement("Slam"));
                }
            }
        }
    }
}
