using JsTestAdapter.Logging;
using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests
{
    public class TestKarmaLogger : TestLoggerBase
    {
        public TestKarmaLogger(Action<string> logMessage)
        {
            LogMessage = logMessage;
        }

        public Action<string> LogMessage { get; private set; }

        public override void Log(TestLogLevel level, IEnumerable<string> phase, string message)
        {
            LogMessage(FormatMessage(level, phase, message));
        }
    }
}
