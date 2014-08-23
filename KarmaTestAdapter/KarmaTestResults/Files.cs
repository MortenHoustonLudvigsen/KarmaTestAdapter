using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.KarmaTestResults
{
    public class Files : TestCaseContainer
    {
        public Files(Item parent, XElement element)
            : base(parent, element)
        {
        }
    }
}
