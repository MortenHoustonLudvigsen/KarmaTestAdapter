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
    public class FilePattern
    {
        public FilePattern(string pattern, string cwd)
        {
            pattern = pattern.Replace('\\', Path.DirectorySeparatorChar);
            pattern = pattern.Replace('/', Path.DirectorySeparatorChar);
            var segments = pattern.Split(Path.DirectorySeparatorChar).AsEnumerable();
            var fixedSegments = segments.TakeWhile(s => !JokerCharRe.IsMatch(s));
            segments = segments.Skip(fixedSegments.Count());

            Directory = GetFullPath(string.Join(DirectorySeparatorStr, fixedSegments), cwd);
            if (segments.Any())
            {
                Pattern = string.Join(DirectorySeparatorStr, new[] { Directory, string.Join(DirectorySeparatorStr, segments) });
            }
            else
            {
                Pattern = Directory;
                Directory = Path.GetDirectoryName(Directory);
            }
        }

        private static readonly Regex JokerCharRe = new Regex(@"[*?\[\]{}]");
        public string Pattern { get; private set; }
        public string FileFilter { get { return Path.GetFileName(Pattern); } }
        public string Directory { get; private set; }

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

        private static readonly string DirectorySeparatorStr = new string(Path.DirectorySeparatorChar, 1);
        private string Normalize(string pattern, string cwd, out string directory)
        {
            pattern = pattern.Replace('\\', Path.DirectorySeparatorChar);
            pattern = pattern.Replace('/', Path.DirectorySeparatorChar);
            var segments = pattern.Split(Path.DirectorySeparatorChar);
            var fixedSegments = segments.TakeWhile(s => !JokerCharRe.IsMatch(s));
            var rest = segments.Skip(fixedSegments.Count());

            directory = GetFullPath(string.Join(DirectorySeparatorStr, fixedSegments), cwd);
            if (rest.Any())
            {
                return string.Join(DirectorySeparatorStr, new[] { directory, string.Join(DirectorySeparatorStr, rest) });
            }
            pattern = directory;
            directory = Path.GetDirectoryName(directory);
            return pattern;
        }

        private string GetFullPath(string path, string cwd)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                path = ".";
            }
            var oldCwd = System.IO.Directory.GetCurrentDirectory();
            try
            {
                System.IO.Directory.SetCurrentDirectory(cwd);
                return Path.GetFullPath(path);
            }
            finally
            {
                System.IO.Directory.SetCurrentDirectory(oldCwd);
            }
        }
    }
}
