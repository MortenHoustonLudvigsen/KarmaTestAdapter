using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public abstract class Item
    {
        protected Item(Item parent, XElement element)
        {
            Parent = parent;
            _element = element;
            Children = element.GetElements().Select(e => Create(e));
            Name = Attribute("Name") ?? "";
            Root = GetParent<Karma>();
        }

        private XElement _element;

        [JsonIgnore]
        public abstract bool IsValid { get; }

        [JsonIgnore]
        public Item Parent { get; private set; }

        [JsonIgnore]
        public IEnumerable<Item> Children { get; private set; }

        public string Name { get; protected set; }

        [JsonIgnore]
        public Karma Root { get; protected set; }

        protected string GetFullPath(string path)
        {
            if (Root != null && Root.KarmaConfig != null && !string.IsNullOrWhiteSpace(Root.KarmaConfig.BasePath))
            {
                return PathUtils.GetFullPath(path, Root.KarmaConfig.BasePath);
            }
            return path;
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

        protected T GetChild<T>()
            where T : Item
        {
            return Children.OfType<T>().FirstOrDefault();
        }

        protected IEnumerable<T> GetChildren<T>()
            where T : Item
        {
            return Children.OfType<T>();
        }

        protected IEnumerable<T> GetAllChildren<T>()
            where T : Item
        {
            return AllChildren.OfType<T>();
        }

        [JsonIgnore]
        private IEnumerable<Item> AllChildren
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
            return _element.GetAttributeValue(name);
        }

        protected string ValueOfElement(string name = null)
        {
            return _element.GetElementValue(name);
        }

        protected IEnumerable<XElement> Elements()
        {
            return _element.GetElements();
        }

        protected IEnumerable<XElement> Elements(XName name)
        {
            return _element.GetElements(name);
        }
    }
}
