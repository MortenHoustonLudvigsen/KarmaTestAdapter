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

        public override void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            Log(GetMessageLevel(testMessageLevel), message);
        }

        public override void Clear()
        {
            try
            {
                Logger.Clear();
            }
            catch { }
        }

        public override void Log(MessageLevel messageLevel, string message)
        {
            try
            {
                Logger.Log(messageLevel, FormatMessage(messageLevel, message));
            }
            catch { }
        }
    }
}
