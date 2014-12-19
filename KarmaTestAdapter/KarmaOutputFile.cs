using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public class KarmaOutputFile : IDisposable
    {
        public KarmaOutputFile(string outputDirectory, string filename)
            : this(GetPath(outputDirectory, filename), string.IsNullOrWhiteSpace(outputDirectory))
        {
        }

        public KarmaOutputFile(string path, bool isTemporary)
        {
            Path = path;
            IsTemporary = isTemporary;
        }

        public string Path { get; private set; }
        public bool IsTemporary { get; private set; }

        private static string GetPath(string outputDirectory, string filename)
        {
            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                if (!System.IO.Directory.Exists(outputDirectory))
                {
                    System.IO.Directory.CreateDirectory(outputDirectory);
                }
                return System.IO.Path.Combine(outputDirectory, filename);
            }
            else
            {
                return System.IO.Path.GetTempFileName();
            }
        }

        public void Dispose()
        {
            if (IsTemporary)
            {
                File.Delete(Path);
            }
        }
    }
}
