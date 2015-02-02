using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using Newtonsoft.Json;
using Summerset.SemanticVersion;
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
        public string Command { get; protected set; }
        public string Directory { get; private set; }
        public virtual string Name { get { return Command; } }

        private Process _process;
        private static readonly ISemanticVersion _minVersion = new SemanticVersion("0.7.0");

        protected ProcessOptions GetProcessOptions(KarmaSettings settings)
        {
            var karmaVsReporter = FindKarmaVsReporter(Directory);
            if (!karmaVsReporter.Exists)
            {
                throw new Exception("Could not find node module karma-vs-reporter");
            }

            if (karmaVsReporter.Version == null)
            {
                throw new Exception("Could not find node module karma-vs-reporter version");
            }

            if (karmaVsReporter.Version.CompareTo(_minVersion) < 0)
            {
                throw new Exception(string.Format("Installed version of node module karma-vs-reporter ({0}) is less than the required version ({1})", karmaVsReporter.Version, _minVersion));
            }

            var processOptions = new ProcessOptions("node", PathUtils.GetRelativePath(Directory, karmaVsReporter.Reporter, true), Command)
            {
                WorkingDirectory = Directory,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            if (IO.File.Exists(settings.SettingsFile))
            {
                processOptions.AddFileOption("-c", settings.SettingsFile);
            }

            return processOptions;
        }

        public void Cancel(IKarmaLogger logger)
        {
            if (_process != null)
            {
                _process.Cancel();
                if (logger != null)
                {
                    logger.Info("{0} cancelled", Command);
                }
            }
            else if (logger != null)
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

        private static Dictionary<string, KarmaVsReporter> _karmaVsReporters = new Dictionary<string, KarmaVsReporter>(StringComparer.OrdinalIgnoreCase);
        private static object _karmaVsReportersLock = new object();
        private static KarmaVsReporter FindKarmaVsReporter(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return null;
            }

            lock (_karmaVsReportersLock)
            {
                KarmaVsReporter reporter;
                if (!_karmaVsReporters.TryGetValue(directory, out reporter))
                {
                    reporter = new KarmaVsReporter(directory);
                    _karmaVsReporters.Add(directory, reporter);
                }
                return reporter;
            }
        }
    }
}
