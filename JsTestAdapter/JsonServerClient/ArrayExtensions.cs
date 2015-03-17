using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.JsonServerClient
{
    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int length)
        {
            return data.SubArray(0, length);
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            length = Math.Min(data.Length - index, length);
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
