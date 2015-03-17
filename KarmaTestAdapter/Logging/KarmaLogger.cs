using JsTestAdapter.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public class KarmaLogger : TestLogger
    {
        private readonly List<ITestLogger> _loggers = new List<ITestLogger>();

        public KarmaLogger(ILogger logger, bool newGlobalLog = false)
            : this(logger, null, newGlobalLog)
        {
        }

        public KarmaLogger(IMessageLogger messageLogger, bool newGlobalLog = false)
            : this(null, messageLogger, newGlobalLog)
        {
        }

        private KarmaLogger(ILogger logger, IMessageLogger messageLogger, bool newGlobalLog)
        {
            if (Globals.Debug)
            {
                if (!Directory.Exists(Globals.GlobalLogDir))
                {
                    Directory.CreateDirectory(Globals.GlobalLogDir);
                }
                if (newGlobalLog && File.Exists(Globals.GlobalLogFile))
                {
                    File.Delete(Globals.GlobalLogFile);
                }
                this.Info("Logging to {0}", Globals.GlobalLogFile);
                this.AddLogger(Globals.GlobalLogFile);
            }
            this.AddLogger(logger);
            this.AddLogger(messageLogger);
        }
    }
}
