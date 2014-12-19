using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Helpers
{
    public static class Sha1Utils
    {
        public static string GetHash(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            using (var bs = new BufferedStream(fs))
            using (var sha1 = new SHA1Managed())
            {
                return string.Join("", sha1.ComputeHash(bs).Select(b => b.ToString("x2")));
            }
        }
    }
}
