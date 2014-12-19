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

        public IDisposable LogProcess(string message)
        {
            return new ProcessLogger(this, message);
        }

        public IDisposable LogProcess(string fmt, params object[] args)
        {
            return LogProcess(string.Format(fmt, args));
        }

        private class ProcessLogger : IDisposable
        {
            public ProcessLogger(KarmaCommandLogger logger, string message)
            {
                Logger = logger;
                Message = message;
                Logger.Info("Start {0}", Message);
            }

            public KarmaCommandLogger Logger { get; private set; }
            public string Message { get; private set; }

            public void Dispose()
            {
                Logger.Info("Finished {0}", Message);
            }
        }
    }
}
