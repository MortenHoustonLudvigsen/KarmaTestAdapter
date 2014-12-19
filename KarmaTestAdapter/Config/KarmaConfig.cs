using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public class KarmaConfig
    {
        public static KarmaConfig Read(string path)
        {
            var config = JsonConvert.DeserializeObject<KarmaConfig>(File.ReadAllText(path, Encoding.UTF8));
            config.ExcludedFiles = config.Exclude.Select(p => new KarmaConfigExclude { Pattern = p }).ToList();
            return config;
        }

        public KarmaConfigFile[] Files { get; set; }
        public string[] Exclude { get; set; }

        public IEnumerable<KarmaConfigExclude> ExcludedFiles { get; private set; }

        public IEnumerable<string> GetFiles()
        {
            return Files.SelectMany(f => f.GetFiles()).Except(ExcludedFiles.SelectMany(f => f.GetFiles())).Distinct().ToList();
        }

        public bool HasFile(string file)
        {
            return Files.IsMatch(file) && !ExcludedFiles.IsMatch(file);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
