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
                [Fact(DisplayName = "Parent should be null")]
                public void ParentShouldBeNull()
                {
                    Assert.Null(Item.Parent);
                }
            }

            partial class WithoutParents
            {
                [Fact(DisplayName = @"Parent should be null")]
                public void ParentShouldBeNull()
                {
                    Assert.Null(Item.Parent);
                }
            }

            partial class WithParents
            {
                [Fact(DisplayName = @"Parent should not be null")]
                public void ParentShouldNotBeNull()
                {
                    Assert.NotNull(Item.Parent);
                }

                [Fact(DisplayName = @"Parent should be a Parent3 item")]
                public void ParentShouldBeParent3()
                {
                    Assert.IsType<Helper.Parent3>(Item.Parent);
                }
            }
        }
    }
}
