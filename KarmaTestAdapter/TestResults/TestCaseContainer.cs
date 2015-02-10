using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public abstract class TestCaseContainer : TestItem
    {
        public TestCaseContainer(Item parent, XElement element)
            : base(parent, element)
        {
            Suites = GetChildren<Suite>();
            Tests = GetChildren<Test>();
            AllTests = GetAllChildren<Test>();
        }

        public virtual IEnumerable<Suite> Suites { get; private set; }
        public virtual IEnumerable<Test> Tests { get; private set; }

        [JsonIgnore]
        public virtual IEnumerable<Test> AllTests { get; private set; }
    }
}
