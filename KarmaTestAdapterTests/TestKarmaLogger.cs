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
        public TestKarmaLogger(Action<string> logMessage)
        {
            LogMessage = logMessage;
        }

        public Action<string> LogMessage { get; private set; }

        public override void Log(KarmaLogLevel level, IEnumerable<string> phase, string message)
        {
            LogMessage(FormatMessage(level, phase, message));
        }
    }
}
