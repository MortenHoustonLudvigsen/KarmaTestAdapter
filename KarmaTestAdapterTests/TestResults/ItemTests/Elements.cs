using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.ItemTests
{
    partial class TestResults
    {
        partial class Item
        {
            partial class Empty
            {
                [Fact(DisplayName = @"Elements() should return empty list")]
                public void Elements_ShouldReturnNull()
                {
                    Assert.Empty(Item.Elements());
                }

                [Fact(DisplayName = @"Elements(""Slam"") should return empty list")]
                public void Elements_Slam_ShouldReturnNull()
                {
                    Assert.Empty(Item.Elements());
                }
            }

            partial class WithElement
            {
                [Fact(DisplayName = @"Elements() should return [<Slam>Bam</Slam>]")]
                public void Elements_ShouldReturnListWithSlamElement()
                {
                    var expected = new[] { new XElement("Slam", "Bam") }.AsEnumerable();
                    Assert.Equal(expected, Item.Elements(), XElementComparer.Instance);
                }

                [Fact(DisplayName = @"Elements(""Slam"") should return [<Slam>Bam</Slam>]")]
                public void Elements_Slam_ShouldReturnListWithSlamElement()
                {
                    var expected = new[] { new XElement("Slam", "Bam") }.AsEnumerable();
                    Assert.Equal(expected, Item.Elements("Slam"), XElementComparer.Instance);
                }
            }
        }
    }
}
