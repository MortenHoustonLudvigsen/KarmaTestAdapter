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
        private IMessageLogger _logger;

        public KarmaTestMessageLogger(IMessageLogger logger)
        {
            _logger = logger;
        }

        public override void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            try
            {
                _logger.SendMessage(testMessageLevel, FormatMessage(testMessageLevel, message));
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
