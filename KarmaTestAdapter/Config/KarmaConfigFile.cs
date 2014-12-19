using Minimatch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Config
{
    public class KarmaConfigFile : KarmaConfigFileBase
    {
        public bool Served { get; set; }
        public bool Included { get; set; }
        public bool Watched { get; set; }
    }
}
