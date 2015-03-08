using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestContainerSource
    {
        public KarmaTestContainerSource(string karmaConfigFile)
        {
            KarmaConfigFile = karmaConfigFile;
        }

        public string KarmaConfigFile { get; set; }
        public int Port { get; set; }
    }
}