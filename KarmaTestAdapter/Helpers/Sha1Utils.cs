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
            return GetHashInternal(path);
            //return GetHash(path, 5, 10).Result;
        }

        private static string GetHashInternal(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            using (var fs = PathUtils.OpenRead(path))
            using (var bs = new BufferedStream(fs))
            using (var sha1 = new SHA1Managed())
            {
                return string.Join("", sha1.ComputeHash(bs).Select(b => b.ToString("x2")));
            }
        }

        private async static Task<string> GetHash(string path, int attempts, int retryDelay)
        {
            try
            {
                return GetHashInternal(path);
            }
            catch (IOException ex)
            {
                if (attempts <= 0)
                {
                    throw ex;
                }
            }
            await Task.Delay(TimeSpan.FromMilliseconds(retryDelay));
            return GetHash(path, attempts - 1, retryDelay).Result;
        }
    }
}
