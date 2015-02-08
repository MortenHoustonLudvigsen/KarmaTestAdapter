using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public class Results : ResultContainer
    {
        public Results(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public virtual IEnumerable<Browser> Browsers { get { return Children.OfType<Browser>(); } }
    }
}
