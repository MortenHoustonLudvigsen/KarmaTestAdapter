using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.VsConfig
{
    public class File
    {
        public string path { get; set; }
        public bool? served { get; set; }
        public bool? included { get; set; }
        public IEnumerable<Test> tests { get; set; }
    }
}
