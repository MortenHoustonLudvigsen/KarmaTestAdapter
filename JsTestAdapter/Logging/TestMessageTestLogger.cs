using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Logging
{
    public class TestMessageTestLogger : TestLoggerBase
    {
        public IMessageLogger Logger { get; private set; }

        public TestMessageTestLogger(IMessageLogger logger)
        {
            Logger = logger;
        }

        public override void Clear()
        {
        }

        protected TestMessageLevel? GetTestMessageLevel(TestLogLevel level)
        {
            switch (level)
            {
                case TestLogLevel.Informational:
                    return TestMessageLevel.Informational;
                case TestLogLevel.Warning:
                    return TestMessageLevel.Warning;
                case TestLogLevel.Error:
                    return TestMessageLevel.Error;
                default:
                    return null;
            }
        }

        public override void Log(TestLogLevel level, IEnumerable<string> context, string message)
        {
            try
            {
                var testMessageLevel = GetTestMessageLevel(level);
                if (testMessageLevel.HasValue)
                {
                    Logger.SendMessage(testMessageLevel.Value, FormatMessage(level, context, message));
                }
            }
            catch { }
        }
    }
}
