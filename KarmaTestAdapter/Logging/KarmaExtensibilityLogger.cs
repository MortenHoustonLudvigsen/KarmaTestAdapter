using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public class KarmaExtensibilityLogger : KarmaLoggerBase
    {
        public KarmaExtensibilityLogger(ILogger logger)
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

        protected MessageLevel? GetMessageLevel(KarmaLogLevel level)
        {
            switch (level)
            {
                case KarmaLogLevel.Informational:
                    return MessageLevel.Informational;
                case KarmaLogLevel.Warning:
                    return MessageLevel.Warning;
                case KarmaLogLevel.Error:
                    return MessageLevel.Error;
                default:
                    return null;
            }
        }

        public override void Log(KarmaLogLevel level, IEnumerable<string> context, string message)
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
