using Minimatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public class KarmaConfigFileBase
    {
        public string Pattern { get; set; }

        private Minimatcher _matcher = null;
        public Minimatcher Matcher { get { return _matcher ?? new Minimatcher(Pattern, new Options { NoCase = true }); } }

        public bool IsMatch(string file)
        {
            return Matcher.IsMatch(file.Replace('\\', '/'));
        }

        public IEnumerable<string> GetFiles()
        {
            return Glob.Glob.ExpandNames(Pattern, ignoreCase: true);
        }
    }
}
