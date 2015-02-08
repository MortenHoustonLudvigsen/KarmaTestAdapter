using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.Item
{
    partial class TestResults
    {
        partial class Item
        {
            partial class Empty
            {
                [Fact(DisplayName = @"Attribute(""Path"") should return null")]
                public void Attribute_Path_ShouldReturnNull()
                {
                    Assert.Null(Item.Attribute("Path"));
                }
            }

            partial class WithElement
            {
                [Fact(DisplayName = @"Attribute(""Path"") should return ""The path""")]
                public void Attribute_Path_ShouldReturnThePath()
                {
                    Assert.Equal("The path", Item.Attribute("Path"));
                }
            }
        }
    }
}
