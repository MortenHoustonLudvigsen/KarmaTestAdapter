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
                [Fact(DisplayName = "Name should be null")]
                public void NameShouldBeNull()
                {
                    Assert.Null(Item.Name);
                }
            }
        }
    }
}
