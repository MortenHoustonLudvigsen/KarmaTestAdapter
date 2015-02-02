using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public class KarmaConfig : FilesSpec
    {
        public KarmaConfig(string path)
        {
            var karmaConfig = Json.ReadFile(path, new
            {
                Files = new[] { new {
                    Pattern = string.Empty,
                    Served = string.Empty,
                    Included = string.Empty,
                    Watched = string.Empty
                } },
                Exclude = new[] { string.Empty }
            });

            var cwd = Path.GetFullPath(Path.GetDirectoryName(path));

            Include(cwd, karmaConfig.Files.Select(f => f.Pattern));
            Exclude(cwd, karmaConfig.Exclude);
        }

        public override string ToString()
        {
            return Json.Serialize(this);
        }
    }
}
