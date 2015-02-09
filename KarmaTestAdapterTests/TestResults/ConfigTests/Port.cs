using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.ConfigTests
{
    partial class TestResults
    {
        partial class Config
        {
            partial class Empty
            {
                [Fact(DisplayName = "Port should be null")]
                public void PortShouldBeNull()
                {
                    Assert.Null(Item.Port);
                }
            }

            [Fact(DisplayName = "Port should not be null")]
            public void PortShouldNotBeNull()
            {
                Assert.NotNull(Item.Port);
            }

            [Fact(DisplayName = "Port should be correct")]
            public void PortShouldBeCorrect()
            {
                Assert.Equal(53983, Item.Port);
            }
        }
    }
}
