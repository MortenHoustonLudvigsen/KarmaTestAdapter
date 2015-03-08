using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests
{
    public class TestKarmaLogger : KarmaLoggerBase
    {
        public TestKarmaLogger(Action<KarmaLogLevel, string, string> logMessage)
        {
            LogMessage = logMessage;
        }

        public Action<KarmaLogLevel, string, string> LogMessage { get; private set; }

        public override void Log(KarmaLogLevel level, string phase, string message)
        {
            LogMessage(level, phase, message);
        }
    }
}
