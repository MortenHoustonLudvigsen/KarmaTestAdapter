using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapter.TestResults
{
    public class Files : TestCaseContainer, IEnumerable<File>
    {
        public Files(Item parent, XElement element)
            : base(parent, element)
        {
        }

        public override bool IsValid
        {
            get { return true; }
        }

        public IEnumerator<File> GetEnumerator()
        {
            return GetChildren<File>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
