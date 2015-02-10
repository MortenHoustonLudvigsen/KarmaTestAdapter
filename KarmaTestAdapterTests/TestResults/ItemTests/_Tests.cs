using KarmaTestAdapter.TestResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace KarmaTestAdapterTests.TestResults.ItemTests
{
    public partial class TestResults
    {
        public abstract class ItemTestsHelper : Helper<ItemTestsHelper.ConcreteItem, KarmaTestAdapter.TestResults.Item>
        {
            public override KarmaTestAdapter.TestResults.Item CreateParent()
            {
                return null;
            }

            public virtual XElement GetElement()
            {
                return null;
            }

            public override ConcreteItem CreateItem()
            {
                return new ConcreteItem(CreateParent(), GetElement());
            }

            public class ConcreteItem : KarmaTestAdapter.TestResults.Item
            {
                public ConcreteItem(KarmaTestAdapter.TestResults.Item parent, XElement element)
                    : base(parent, element)
                {
                }

                public new KarmaTestAdapter.TestResults.Item Create(XElement element)
                {
                    return base.Create(element);
                }

                public new T GetParent<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetParent<T>();
                }

                public new T GetChild<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetChild<T>();
                }

                public new IEnumerable<T> GetChildren<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetChildren<T>();
                }

                public new IEnumerable<T> GetAllChildren<T>()
                    where T : KarmaTestAdapter.TestResults.Item
                {
                    return base.GetAllChildren<T>();
                }

                public new string Attribute(string name)
                {
                    return base.Attribute(name);
                }

                public new string ValueOfElement(string name = null)
                {
                    return base.ValueOfElement(name);
                }

                public new IEnumerable<XElement> Elements()
                {
                    return base.Elements();
                }

                public new IEnumerable<XElement> Elements(XName name)
                {
                    return base.Elements(name);
                }
            }
        }

        public partial class Item : ItemTestsHelper.Tests<Item.Helper>
        {
            public class Helper : ItemTestsHelper
            {
            }

            public partial class Empty : ItemTestsHelper.Tests<Helper>
            {
            }

            public partial class WithParents : ItemTestsHelper.Tests<WithParents.Helper>
            {
                public class Helper : ItemTestsHelper
                {
                    public class Parent1 : ConcreteItem
                    {
                        public Parent1() : base(new Karma((XElement)null), null) { }
                    }

                    public class Parent2 : ConcreteItem
                    {
                        public Parent2() : base(new Parent1(), null) { }
                    }

                    public class Parent3 : ConcreteItem
                    {
                        public Parent3() : base(new Parent2(), null) { }
                    }

                    public override KarmaTestAdapter.TestResults.Item CreateParent()
                    {
                        return new Parent3();
                    }
                }
            }

            public partial class WithoutParents : ItemTestsHelper.Tests<Helper>
            {
            }

            public partial class WithElement : ItemTestsHelper.Tests<WithElement.Helper>
            {
                public class Helper : ItemTestsHelper
                {
                    private XElement _element = new XElement("Karma",
                        new XAttribute("Path", "The path"),
                        new XElement("Slam", "Bam")
                    );

                    public override XElement GetElement()
                    {
                        return _element;
                    }
                }
            }

            public partial class WithChildren : ItemTestsHelper.Tests<WithChildren.Helper>
            {
                public class Helper : ItemTestsHelper
                {
                    public override XElement GetElement()
                    {
                        return XDocument.Parse(Constants.KarmaXml).Root;
                    }
                }
            }
        }
    }
}