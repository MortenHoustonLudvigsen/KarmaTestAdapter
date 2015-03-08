using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public class KarmaTestMessageLogger : KarmaLoggerBase
    {
        public IMessageLogger Logger { get; private set; }

        public KarmaTestMessageLogger(IMessageLogger logger)
        {
            Logger = logger;
        }

        public override void Clear()
        {
        }

        protected TestMessageLevel? GetTestMessageLevel(KarmaLogLevel level)
        {
            switch (level)
            {
                case KarmaLogLevel.Informational:
                    return TestMessageLevel.Informational;
                case KarmaLogLevel.Warning:
                    return TestMessageLevel.Warning;
                case KarmaLogLevel.Error:
                    return TestMessageLevel.Error;
                default:
                    return null;
            }
        }

        public override void Log(KarmaLogLevel level, string phase, string message)
        {
            try
            {
                var testMessageLevel = GetTestMessageLevel(level);
                if (testMessageLevel.HasValue)
                {
                    Logger.SendMessage(testMessageLevel.Value, FormatMessage(level, phase, message));
                }
            }
            catch { }
        }
    }
}
