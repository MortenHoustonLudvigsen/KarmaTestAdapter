using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public static class HashSetExtensions
    {
        public static int Add<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            return items.Select(f => set.Add(f)).Count(x => x);
        }

        public static int Remove<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            return items.Select(f => set.Remove(f)).Count(x => x);
        }
    }
}
