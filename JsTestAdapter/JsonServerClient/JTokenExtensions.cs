using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.JsonServerClient
{
    public static class JTokenExtensions
    {
        public static T GetValue<T>(this JToken token, string propertyName)
        {
            return token.GetValue(propertyName, default(T));
        }

        public static T GetValue<T>(this JToken token, string propertyName, T defaultValue)
        {
            try
            {
                return token.Value<T>(propertyName);
            }
            catch (System.FormatException)
            {
                return defaultValue;
            }
        }
    }
}
