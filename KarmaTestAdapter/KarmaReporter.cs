using KarmaTestAdapter.Commands;
using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TwoPS.Processes;
using IO = System.IO;

namespace KarmaTestAdapter
{
    public class KarmaReporter
    {
        public static Karma Discover(string source, IKarmaLogger logger)
        {
            return new KarmaDiscoverCommand(source, logger).Run();
        }

        public static Karma Run(string source, VsConfig.Config vsConfig, IKarmaLogger logger)
        {
            return new KarmaRunCommand(source, vsConfig, logger).Run();
        }
    }
}
