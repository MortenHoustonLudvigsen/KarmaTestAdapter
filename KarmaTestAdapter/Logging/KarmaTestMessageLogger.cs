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

        public override void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            try
            {
                Logger.SendMessage(testMessageLevel, FormatMessage(testMessageLevel, message));
            }
            catch { }
        }

        public override void Clear()
        {
        }

        public override void Log(MessageLevel messageLevel, string message)
        {
            SendMessage(GetTestMessageLevel(messageLevel), message);
        }
    }
}
