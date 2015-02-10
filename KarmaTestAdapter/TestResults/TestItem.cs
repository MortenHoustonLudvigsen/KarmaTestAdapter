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
            Source = GetChild<Source>();
            File = GetParent<File>();
            ParentSuite = GetParent<Suite>();
        }

        public Source Source { get; private set; }

        [JsonIgnore]
        public File File { get; private set; }

        [JsonIgnore]
        public Suite ParentSuite { get; private set; }

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
