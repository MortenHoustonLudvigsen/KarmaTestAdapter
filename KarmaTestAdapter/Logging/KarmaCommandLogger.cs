using KarmaTestAdapter.Commands;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public class KarmaCommandLogger : KarmaLogger
    {
        public KarmaCommandLogger(IKarmaLogger logger, KarmaCommand command)
            : base(logger)
        {
            Command = command;
        }

        public KarmaCommand Command { get; private set; }
        public override string Phase { get { return Command.Name; } }
    }
}
