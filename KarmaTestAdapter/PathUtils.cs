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
            if (string.IsNullOrWhiteSpace(path1) || string.IsNullOrWhiteSpace(path2))
            {
                return false;
            }
            return string.Equals(Path.GetFullPath(path1), Path.GetFullPath(path2), StringComparison.OrdinalIgnoreCase);
        }

        public static bool PathHasFileName(string path, string fileName)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }
            return string.Equals(Path.GetFileName(path), fileName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsInDirectory(string path, string directory)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }
            return string.Equals(Path.GetDirectoryName(path), directory, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetRelativePath(string basePath, string fullPath, bool onlyLocal)
        {
            if (string.IsNullOrWhiteSpace(basePath) || string.IsNullOrWhiteSpace(fullPath))
            {
                return fullPath ?? "";
            }

            if (Directory.Exists(basePath) && !basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }

            var relativePath = new Uri(basePath)
                .MakeRelativeUri(new Uri(fullPath))
                .ToString()
                .Replace('/', Path.DirectorySeparatorChar);

            if (onlyLocal && relativePath.StartsWith("."))
            {
                return fullPath;
            }

            return relativePath;
        }
    }
}
