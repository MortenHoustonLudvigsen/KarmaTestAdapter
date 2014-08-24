using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
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
            Settings = KarmaSettings.Read(source, logger);
            Directory = IO.Path.GetDirectoryName(IO.Path.GetFullPath(source));
            Logger = logger;
        }

        public KarmaSettings Settings { get; private set; }
        public string Command { get; private set; }
        public string Directory { get; private set; }
        public IKarmaLogger Logger { get; private set; }

        protected virtual ProcessOptions GetProcessOptions()
        {
            var karmaVsReporter = FindKarmaVsReporter(Directory);
            if (string.IsNullOrWhiteSpace(karmaVsReporter))
            {
                throw new Exception("Could not find node module karma-vs-reporter");
            }

            var processOptions = new ProcessOptions("node", karmaVsReporter, Command)
            {
                WorkingDirectory = Directory
            };

            if (!string.IsNullOrWhiteSpace(Settings.SettingsFile))
            {
                processOptions.Add("-c", Settings.SettingsFile);
            }

            return processOptions;
        }

        public virtual Karma Run()
        {
            var outputFile = Globals.LogToFile ? Globals.OutputFilename : IO.Path.GetTempFileName();
            try
            {
                var processOptions = GetProcessOptions();

                processOptions.Add("-o", outputFile);

                Logger.Info(processOptions.CommandLine);
                var result = Process.Run(processOptions);
                foreach (var line in result.AllOutputList)
                {
                    Logger.Info(line);
                }
                return Karma.Load(outputFile);
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
                    IO.File.Delete(outputFile);
                }
            }
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
