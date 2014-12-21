using Minimatch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public class KarmaConfigFileBase
    {
        private static readonly Regex JokerCharRe = new Regex(@"[*?\[\]{}]");
        public string Pattern { get; set; }
        public string FileFilter { get { return Path.GetFileName(Pattern); } }
        public string Directory
        {
            get
            {
                return string.Join(
                    new string(Path.DirectorySeparatorChar, 1),
                    Path.GetDirectoryName(Pattern)
                        .Split(Path.DirectorySeparatorChar)
                        .TakeWhile(p => !JokerCharRe.IsMatch(p))
                );
            }
        }

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
