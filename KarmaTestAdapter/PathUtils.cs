using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public static class PathUtils
    {
        public static bool PathsEqual(string path1, string path2)
        {
            return string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool PathHasFileName(string path, string fileName)
        {
            return string.Equals(Path.GetFileName(path), fileName, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsInDirectory(string path, string directory)
        {
            return string.Equals(Path.GetDirectoryName(path), directory, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
