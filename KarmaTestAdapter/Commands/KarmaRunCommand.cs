using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TwoPS.Processes;
using IO = System.IO;

namespace KarmaTestAdapter.Commands
{
    public class KarmaRunCommand : KarmaCommand
    {
        public KarmaRunCommand(string source, VsConfig.Config vsConfig, IKarmaLogger logger)
            : base("run", source, logger)
        {
            _vsConfig = vsConfig;
        }

        private VsConfig.Config _vsConfig;
        private string _vsConfigFile;

        protected override ProcessOptions GetProcessOptions()
        {
            var processOptions = base.GetProcessOptions();
            processOptions.Add("-p", GetFreeTcpPort());
            processOptions.Add("-v", _vsConfigFile);
            return processOptions;
        }

        public override Karma Run()
        {
            _vsConfigFile = Globals.LogToFile ? Globals.VsConfigFilename : IO.Path.GetTempFileName();
            try
            {
                IO.File.WriteAllText(_vsConfigFile, JsonConvert.SerializeObject(_vsConfig, Formatting.Indented));
                return base.Run();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
            finally
            {
                if (!Globals.LogToFile)
                {
                    IO.File.Delete(_vsConfigFile);
                }
                _vsConfigFile = null;
            }
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
