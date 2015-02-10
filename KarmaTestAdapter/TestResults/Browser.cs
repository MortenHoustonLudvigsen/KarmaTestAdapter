using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public class Browser: ResultContainer
    {
        public Browser(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public override bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(Name); }
        }
    }
}
