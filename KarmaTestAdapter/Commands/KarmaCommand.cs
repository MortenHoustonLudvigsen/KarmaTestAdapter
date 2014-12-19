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
        public KarmaCommand(string command, string source, IKarmaLogger logger)
        {
            Command = command;
            Logger = new KarmaCommandLogger(logger, this);
            Source = IO.Path.GetFullPath(source);
            Settings = KarmaSettings.Read(Source, Logger);
            Directory = IO.Path.GetDirectoryName(Source);
        }

        public string Source { get; private set; }
        public KarmaSettings Settings { get; private set; }
        public string Command { get; private set; }
        public string Directory { get; private set; }
        public IKarmaLogger Logger { get; private set; }
        public virtual string Name { get { return Command; } }

        private Process _process;

        protected virtual ProcessOptions GetProcessOptions()
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

            if (!string.IsNullOrWhiteSpace(Settings.SettingsFile))
            {
                if (!IO.File.Exists(Settings.SettingsFile))
                {
                    throw new Exception(string.Format("Could not find settings file \"{0}\"", Settings.SettingsFile));
                }
                processOptions.Add("-c", Settings.SettingsFile);
            }

            return processOptions;
        }

        public void Cancel()
        {
            if (_process != null)
            {
                _process.Cancel();
                Logger.Info("{0} cancelled", Command);
            }
            else
            {
                Logger.Info("Tried to cancel {0}, but it was not running", Command);
            }
        }

        public Karma Run()
        {
            var outputDirectory = Settings.GetOutputDirectory(Command);
            return RunInternal(outputDirectory);
        }

        protected virtual Karma RunInternal(string outputDirectory)
        {
            var outputFile = Settings.GetOutputFile(outputDirectory);
            try
            {
                var processOptions = GetProcessOptions();

                processOptions.Add("-o", outputFile);

                Logger.Info(processOptions.CommandLine);

                _process = new Process(processOptions);
                try
                {
                    _process.StandardOutputRead += (o, e) =>
                    {
                        Logger.Info(e.Line);
                    };

                    _process.StandardErrorRead += (o, e) =>
                    {
                        Logger.Info(e.Line);
                    };

                    _process.Run();
                }
                finally
                {
                    _process = null;
                }

                var karma = Karma.Load(outputFile);
                return karma;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
            finally
            {
                if (!Settings.LogToFile)
                {
                    IO.File.Delete(outputFile);
                }
            }
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
