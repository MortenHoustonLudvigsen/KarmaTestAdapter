using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public static class FilePatternExtensions
    {
        public static bool IsMatch(this IEnumerable<FilePattern> patterns, string file)
        {
            foreach (var pattern in patterns)
            {
                if (pattern.IsMatch(file))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<string> GetFiles(this IEnumerable<FilePattern> patterns)
        {
            return patterns
                .SelectMany(f => f.GetFiles())
                .Distinct(StringComparer.OrdinalIgnoreCase);
        }
    }
}
