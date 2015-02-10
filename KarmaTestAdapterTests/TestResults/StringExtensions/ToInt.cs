using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.StringExtensions
{
    public partial class TestResults
    {
        public partial class StringExtensions
        {
            public partial class ToInt
            {
                [Fact(DisplayName = "(null) should return null")]
                public void NullShouldReturnNull()
                {
                    Assert.Null(((string)null).ToInt());
                }

                [Fact(DisplayName = @"("""") should return null")]
                public void EmptyStringShouldReturnNull()
                {
                    Assert.Null("".ToInt());
                }

                [Fact(DisplayName = @"(""  "") should return null")]
                public void WhiteSpaceShouldReturnNull()
                {
                    Assert.Null("  ".ToInt());
                }

                [Fact(DisplayName = @"(""42"") should return 42")]
                public void FortyTwoShouldReturnFortyTwo()
                {
                    Assert.Equal(42, "42".ToInt());
                }

                [Fact(DisplayName = @"(""-42"") should return -42")]
                public void MinusFortyTwoShouldReturnFortyTwo()
                {
                    Assert.Equal(-42, "-42".ToInt());
                }

                [Fact(DisplayName = @"(""abc"") should return null")]
                public void AbcShouldReturnNull()
                {
                    Assert.Null("abc".ToInt());
                }
            }
        }
    }
}
