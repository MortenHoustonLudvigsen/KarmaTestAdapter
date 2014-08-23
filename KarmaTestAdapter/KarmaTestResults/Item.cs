using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.KarmaTestResults
{
    public abstract class Item
    {
        public static IKarmaLogger Logger { get; set; }

        static Item()
        {
            Logger = KarmaLogger.Create();
        }

        protected Item(Item parent, XElement element)
        {
            Parent = parent;
            Element = element;
            Children = element.Elements().Select(e => Create(e));
        }

        private XElement Element;
        public Item Parent { get; private set; }
        public IEnumerable<Item> Children { get; private set; }
        public virtual string Name { get { return Attribute("Name"); } }

        public Karma Root
        {
            get { return GetParent<Karma>(); }
        }

        protected Item Create(XElement element)
        {
            switch (element.Name.LocalName)
            {
                case "Karma":
                    return new Karma(element);
                case "Config":
                    return new Config(this, element);
                case "Files":
                    return new Files(this, element);
                case "File":
                    return new File(this, element);
                case "Suite":
                    return new Suite(this, element);
                case "Test":
                    return new Test(this, element);
                case "Source":
                    return new Source(this, element);
                case "Results":
                    return new Results(this, element);
                case "Browser":
                    return new Browser(this, element);
                case "SuiteResult":
                    return new SuiteResult(this, element);
                case "TestResult":
                    return new TestResult(this, element);
                default:
                    return new UnknownItem(this, element);
            }
        }

        protected T GetParent<T>()
            where T : Item
        {
            if (Parent == null)
            {
                return null;
            }
            if (Parent is T)
            {
                return (T)Parent;
            }
            return Parent.GetParent<T>();
        }

        public IEnumerable<Item> AllChildren
        {
            get
            {
                foreach (var child in Children)
                {
                    yield return child;
                    foreach (var grandChild in child.AllChildren)
                    {
                        yield return grandChild;
                    }
                }
            }
        }

        protected string Attribute(string name)
        {
            return Element.GetAttributeValue(name);
        }

        protected string ValueOfElement(string name = null)
        {
            return Element.GetElementValue(name);
        }

        protected IEnumerable<XElement> Elements()
        {
            return Element.Elements();
        }

        protected IEnumerable<XElement> Elements(XName name)
        {
            return Element.Elements(name);
        }
    }
}
