using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Logging
{
    public class ExtensibilityTestLogger : TestLoggerBase
    {
        public ExtensibilityTestLogger(ILogger logger)
        {
            Logger = logger;
        }

        public ILogger Logger { get; private set; }

        public override void Clear()
        {
            try
            {
                Logger.Clear();
            }
            catch { }
        }

        protected MessageLevel? GetMessageLevel(TestLogLevel level)
        {
            switch (level)
            {
                case TestLogLevel.Informational:
                    return MessageLevel.Informational;
                case TestLogLevel.Warning:
                    return MessageLevel.Warning;
                case TestLogLevel.Error:
                    return MessageLevel.Error;
                default:
                    return null;
            }
        }

        public override void Log(TestLogLevel level, IEnumerable<string> context, string message)
        {
            try
            {
                var messageLevel = GetMessageLevel(level);
                if (messageLevel.HasValue)
                {
                    Logger.Log(messageLevel.Value, FormatMessage(level, context, message));
                }
            }
            catch { }
        }
    }
}
