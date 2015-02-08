using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.TestResults
{
    public static class StringExtensions
    {
        public static int? ToInt(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                int result;
                if (int.TryParse(value, out result))
                {
                    return result;
                }
            }
            return null;
        }

        public static bool? ToBool(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                bool result;
                if (bool.TryParse(value, out result))
                {
                    return result;
                }
            }
            return null;
        }
    }
}
