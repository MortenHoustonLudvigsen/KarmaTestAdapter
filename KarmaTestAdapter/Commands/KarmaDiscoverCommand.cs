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
        public KarmaDiscoverCommand(string source, IKarmaLogger logger)
            : base("discover", source, logger)
        {
        }

        public override string Name { get { return "Discover"; } }

        protected override KarmaTestResults.Karma RunInternal(string outputDirectory)
        {
            Logger.Info("Start ({0})", Source);
            try
            {
                return base.RunInternal(outputDirectory);
            }
            finally
            {
                Logger.Info("Finished ({0})", Source);
            }
        }
    }
}
