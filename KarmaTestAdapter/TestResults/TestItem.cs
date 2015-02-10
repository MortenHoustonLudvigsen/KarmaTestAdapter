using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public abstract class TestItem : Item
    {
        public TestItem(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public Source Source { get { return GetChild<Source>(); } }

        [JsonIgnore]
        public File File { get { return GetParent<File>(); } }

        [JsonIgnore]
        public Suite ParentSuite { get { return GetParent<Suite>(); } }

        public virtual string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return null;
                }
                return ParentSuite != null ? ParentSuite.DisplayName + " " + Name : Name;
            }
        }
    }
}
