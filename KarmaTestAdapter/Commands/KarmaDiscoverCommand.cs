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
        public KarmaDiscoverCommand(string source, KarmaSettings settings, IKarmaLogger logger)
            : base("discover", source, settings, logger)
        {
        }
    }
}
