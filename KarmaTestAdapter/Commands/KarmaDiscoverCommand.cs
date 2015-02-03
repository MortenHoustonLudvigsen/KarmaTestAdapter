using KarmaTestAdapter.KarmaTestResults;
using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Commands
{
    public class KarmaDiscoverCommand : KarmaCommand
    {
        public KarmaDiscoverCommand(string source)
            : base("discover", source)
        {
        }

        public override string Name { get { return "Discover"; } }

        public Karma Run(IKarmaLogger logger)
        {
            using (var commandLogger = new KarmaCommandLogger(logger, this))
            using (var settings = new KarmaSettings(Source, commandLogger))
            {
                if (settings.AreValid)
                {
                    var outputDirectory = settings.GetOutputDirectory(Command);
                    using (commandLogger.LogProcess("({0})", Source))
                    using (var outputFile = new KarmaOutputFile(outputDirectory, Globals.OutputFilename))
                    {
                        try
                        {
                            var processOptions = GetProcessOptions(settings);
                            processOptions.AddFileOption("-o", outputFile.Path);
                            if (RunCommand(processOptions, commandLogger))
                            {
                                return Karma.Load(outputFile.Path);
                            }
                        }
                        catch (Exception ex)
                        {
                            commandLogger.Error(ex);
                        }
                    }
                }
                return null;
            }
        }
    }
}
