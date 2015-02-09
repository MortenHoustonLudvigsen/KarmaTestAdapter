using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public abstract class TestCaseContainer : Item
    {
        public TestCaseContainer(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public virtual IEnumerable<Suite> Suites { get { return GetChildren<Suite>(); } }
        public virtual IEnumerable<Test> Tests { get { return GetChildren<Test>(); } }
        
        [JsonIgnore]
        public virtual IEnumerable<Test> AllTests { get { return GetAllChildren<Test>(); } }
    }
}
