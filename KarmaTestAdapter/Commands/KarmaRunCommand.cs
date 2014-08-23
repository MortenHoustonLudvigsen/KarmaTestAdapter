using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Commands
{
    public class KarmaRunCommand : KarmaCommand
    {
        public KarmaRunCommand(string source, IKarmaLogger logger)
            : base("run", source, logger)
        {
        }

        protected override TwoPS.Processes.ProcessOptions GetProcessOptions()
        {
            var processOptions = base.GetProcessOptions();
            processOptions.Add("-p", GetFreeTcpPort());
            return processOptions;
        }

        private static string GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            try
            {
                return ((IPEndPoint)l.LocalEndpoint).Port.ToString();
            }
            finally
            {
                l.Stop();
            }
        }
    }
}
