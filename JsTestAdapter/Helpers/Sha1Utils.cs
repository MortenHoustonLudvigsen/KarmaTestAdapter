using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Helpers
{
    public static class Sha1Utils
    {
        public static string GetHash(string value)
        {
            return GetHash(Encoding.UTF8.GetBytes(value));
        }

        public static string GetHash(byte[] bytes)
        {
            using (var sha1 = new SHA1Managed())
            {
                return string.Join("", sha1.ComputeHash(bytes).Select(b => b.ToString("x2")));
            }
        }

        public static string GetHashFromFile(string path)
        {
            return GetHashFromFileInternal(path);
            //return GetHash(path, 5, 10).Result;
        }

        private static string GetHashFromFileInternal(string path)
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

        private async static Task<string> GetHashFromFile(string path, int attempts, int retryDelay)
        {
            try
            {
                return GetHashFromFileInternal(path);
            }
            catch (IOException ex)
            {
                if (attempts <= 0)
                {
                    throw ex;
                }
            }
            await Task.Delay(TimeSpan.FromMilliseconds(retryDelay));
            return GetHashFromFile(path, attempts - 1, retryDelay).Result;
        }
    }
}
