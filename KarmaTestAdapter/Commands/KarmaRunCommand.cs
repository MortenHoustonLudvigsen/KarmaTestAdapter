using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwoPS.Processes;
using IO = System.IO;

namespace KarmaTestAdapter.Commands
{
    public class KarmaRunCommand : KarmaCommand
    {
        public KarmaRunCommand(string source, VsConfig.Config vsConfig)
            : base("run", source)
        {
            _vsConfig = vsConfig;
        }

        private VsConfig.Config _vsConfig;

        public override string Name { get { return "Run"; } }

        public Karma Run(IKarmaLogger logger)
        {
            using (var commandLogger = new KarmaCommandLogger(logger, this))
            using (var settings = new KarmaSettings(Source, commandLogger))
            {
                Command = settings.ServerModeValid ? "served-run" : "run";
                var outputDirectory = settings.GetOutputDirectory(Command);
                using (commandLogger.LogProcess("({0})", Source))
                using (var outputFile = new KarmaOutputFile(outputDirectory, Globals.OutputFilename))
                using (var vsConfigFile = new KarmaOutputFile(outputDirectory, Globals.VsConfigFilename))
                {
                    try
                    {
                        IO.File.WriteAllText(vsConfigFile.Path, JsonConvert.SerializeObject(_vsConfig, Formatting.Indented));
                        var processOptions = GetProcessOptions(settings);
                        processOptions.Add("-p", settings.ServerModeValid ? settings.ServerPort : GetFreeTcpPort());
                        processOptions.AddFileOption("-o", outputFile.Path);
                        processOptions.AddFileOption("-v", vsConfigFile.Path);
                        if (RunCommand(processOptions, commandLogger))
                        {
                            Thread.Sleep(20);
                            return Karma.Load(outputFile.Path);
                        }
                    }
                    catch (Exception ex)
                    {
                        commandLogger.Error(ex);
                    }
                }
                return null;
            }
        }

        private static int? GetFreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            try
            {
                return ((IPEndPoint)l.LocalEndpoint).Port;
            }
            finally
            {
                l.Stop();
            }
        }
    }
}
