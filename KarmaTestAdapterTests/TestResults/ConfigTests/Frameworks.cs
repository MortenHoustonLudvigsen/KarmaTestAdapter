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
                [Fact(DisplayName = "Frameworks should not be null")]
                public void FrameworksShouldNotBeNull()
                {
                    Assert.NotNull(Item.Frameworks);
                }

                [Fact(DisplayName = "Frameworks should be empty")]
                public void FrameworksShouldBeEmpty()
                {
                    Assert.Empty(Item.Frameworks);
                }
            }

            [Fact(DisplayName = "Frameworks should not be null")]
            public void FrameworksShouldNotBeNull()
            {
                Assert.NotNull(Item.Frameworks);
            }

            [Fact(DisplayName = "Frameworks should not be empty")]
            public void FrameworksShouldNotBeEmpty()
            {
                Assert.NotEmpty(Item.Frameworks);
            }

            [Fact(DisplayName = "Frameworks should be correct")]
            public void FrameworksShouldBeCorrect()
            {
                Assert.Equal(new[] { "jasmine" }, Item.Frameworks);
            }
        }
    }
}
