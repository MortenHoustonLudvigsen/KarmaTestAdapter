using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Helpers
{
    public static class Json
    {
        public static T ReadFile<T>(string path, T anonymousTypeObject)
        {
            try
            {
                return Read(File.ReadAllText(path, Encoding.UTF8), anonymousTypeObject);
            }
            catch (Exception ex)
            {
                throw ex.Wrap("Could not read JSON file {0}", path);
            }
        }

        public static T Read<T>(string json, T anonymousTypeObject)
        {
            try
            {
                return JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            }
            catch (Exception ex)
            {
                throw ex.Wrap("Could not read JSON: {0}", json);
            }
        }

        public static void PopulateFromFile(string path, object target)
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(path, Encoding.UTF8), target);
            }
            catch (Exception ex)
            {
                throw ex.Wrap("Could not read JSON file {0}", path);
            }
        }

        public static string Serialize(object value)
        {
            try
            {
                return JsonConvert.SerializeObject(value, Formatting.Indented);
            }
            catch (Exception ex)
            {
                throw ex.Wrap("Could not serialize object to JSON");
            }
        }

        public static void WriteToFile(string path, object value)
        {
            try
            {
                File.WriteAllText(path, Serialize(value));
            }
            catch (Exception ex)
            {
                throw ex.Wrap("Could not write JSON file {0}", path);
            }
        }
    }
}
