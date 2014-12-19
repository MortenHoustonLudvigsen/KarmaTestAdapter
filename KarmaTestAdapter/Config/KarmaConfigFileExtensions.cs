using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public static class KarmaConfigFileExtensions
    {
        public static bool IsMatch(this IEnumerable<KarmaConfigFileBase> configFiles, string file)
        {
            foreach (var configFile in configFiles)
            {
                if (configFile.IsMatch(file))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
