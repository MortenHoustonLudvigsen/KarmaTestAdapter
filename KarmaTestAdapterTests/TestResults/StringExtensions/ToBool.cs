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
            public partial class ToBool
            {
                [Fact(DisplayName = "(null) should return null")]
                public void NullShouldReturnNull()
                {
                    Assert.Null(((string)null).ToBool());
                }

                [Fact(DisplayName = @"("""") should return null")]
                public void EmptyStringShouldReturnNull()
                {
                    Assert.Null("".ToBool());
                }

                [Fact(DisplayName = @"(""  "") should return null")]
                public void WhiteSpaceShouldReturnNull()
                {
                    Assert.Null("  ".ToBool());
                }

                [Fact(DisplayName = @"(""true"") should return true")]
                public void TrueShouldReturnTrue()
                {
                    Assert.Equal(true, "true".ToBool());
                }

                [Fact(DisplayName = @"(""false"") should return false")]
                public void FalseShouldReturnFalse()
                {
                    Assert.Equal(false, "false".ToBool());
                }

                [Fact(DisplayName = @"(""abc"") should return null")]
                public void AbcShouldReturnNull()
                {
                    Assert.Null("abc".ToBool());
                }
            }
        }
    }
}
