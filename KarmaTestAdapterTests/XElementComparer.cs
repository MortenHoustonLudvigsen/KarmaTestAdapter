using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KarmaTestAdapterTests
{
    public class XElementComparer : IEqualityComparer<XElement>
    {
        public static readonly XElementComparer Instance = new XElementComparer();

        public bool Equals(XElement x, XElement y)
        {
            return x.ToString() == y.ToString();
        }

        public int GetHashCode(XElement obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
