using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ude;

namespace KarmaTestAdapter
{
    public static class PathUtils
    {
        public static string GetFullPath(string path, string basePath)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return path;
            }
            var oldCwd = System.IO.Directory.GetCurrentDirectory();
            try
            {
                System.IO.Directory.SetCurrentDirectory(basePath);
                return Path.GetFullPath(path);
            }
            finally
            {
                System.IO.Directory.SetCurrentDirectory(oldCwd);
            }
        }

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

        public static Stream OpenRead(string path)
        {
            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public static string ReadFileText(string path, Encoding defaultEncoding = null)
        {
            using (var fs = OpenRead(path))
            using (var reader = new StreamReader(fs, DetectEncoding(fs, defaultEncoding)))
            {
                return reader.ReadToEnd();
            }
        }

        private static Encoding DetectEncoding(Stream stream, Encoding defaultEncoding = null)
        {
            var encoding = defaultEncoding ?? Encoding.Default;
            try
            {
                var cdet = new CharsetDetector();
                cdet.Feed(stream);
                cdet.DataEnd();
                if (cdet.Charset != null)
                {
                    encoding = Encoding.GetEncoding(cdet.Charset);
                }
            }
            catch (Exception)
            {
                // Do nothing
            }
            finally
            {
                stream.Position = 0;
            }
            return encoding;
        }
    }
}
