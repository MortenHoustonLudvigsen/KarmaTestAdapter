using KarmaTestAdapter.Logging;
using KarmaTestAdapter.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Commands
{
    public class KarmaGetConfigCommand : KarmaCommand
    {
        public static KarmaConfig GetConfig(string source, IKarmaLogger logger)
        {
            return new KarmaGetConfigCommand(source).Run(logger);
        }

        public KarmaGetConfigCommand(string source)
            : base("get-config", source)
        {
        }

        public override string Name { get { return "Get config"; } }

        public KarmaConfig Run(IKarmaLogger logger)
        {
            using (var commandLogger = new KarmaCommandLogger(logger, this))
            using (var settings = new KarmaSettings(Source, commandLogger))
            {
                if (settings.AreValid)
                {
                    var outputDirectory = settings.GetOutputDirectory(Command);
                    using (commandLogger.LogProcess("({0})", Source))
                    using (var outputFile = new KarmaOutputFile(outputDirectory, Globals.ConfigFilename))
                    {
                        try
                        {
                            var processOptions = GetProcessOptions(settings);
                            processOptions.AddFileOption("-o", outputFile.Path);
                            if (RunCommand(processOptions, commandLogger))
                            {
                                return new KarmaConfig(outputFile.Path);
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
