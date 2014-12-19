using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoPS.Processes;
using IO = System.IO;

namespace KarmaTestAdapter.Commands
{
    public class KarmaCommand
    {
        public KarmaCommand(string command, string source)
        {
            Command = command;
            Source = IO.Path.GetFullPath(source);
            Directory = IO.Path.GetDirectoryName(Source);
        }

        public string Source { get; private set; }
        public string Command { get; private set; }
        public string Directory { get; private set; }
        public virtual string Name { get { return Command; } }

        private Process _process;

        protected ProcessOptions GetProcessOptions(KarmaSettings settings)
        {
            var karmaVsReporter = FindKarmaVsReporter(Directory);
            if (string.IsNullOrWhiteSpace(karmaVsReporter))
            {
                throw new Exception("Could not find node module karma-vs-reporter");
            }

            var processOptions = new ProcessOptions("node", karmaVsReporter, Command)
            {
                WorkingDirectory = Directory,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            if (!string.IsNullOrWhiteSpace(settings.SettingsFile))
            {
                if (!IO.File.Exists(settings.SettingsFile))
                {
                    throw new Exception(string.Format("Could not find settings file \"{0}\"", settings.SettingsFile));
                }
                processOptions.Add("-c", settings.SettingsFile);
            }

            return processOptions;
        }

        public void Cancel(IKarmaLogger logger)
        {
            if (_process != null)
            {
                _process.Cancel();
                logger.Info("{0} cancelled", Command);
            }
            else
            {
                logger.Info("Tried to cancel {0}, but it was not running", Command);
            }
        }

        protected bool RunCommand(ProcessOptions processOptions, IKarmaLogger logger)
        {
            try
            {
                logger.Info(processOptions.CommandLine);

                _process = new Process(processOptions);
                try
                {
                    _process.StandardOutputRead += (o, e) =>
                    {
                        logger.Info(e.Line);
                    };

                    _process.StandardErrorRead += (o, e) =>
                    {
                        logger.Info(e.Line);
                    };

                    _process.Run();
                }
                finally
                {
                    _process = null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }
            return true;
        }

        void process_StandardErrorRead(object sender, ProcessEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static string FindKarmaVsReporter(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return null;
            }
            if (!IO.Directory.Exists(directory))
            {
                return null;
            }
            var candidate = IO.Path.Combine(directory, "node_modules", "karma-vs-reporter", "karma-vs-reporter");
            return IO.File.Exists(candidate) ? candidate : null;
        }
    }
}
