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
        private ILogger _logger;

        public KarmaExtensibilityLogger(ILogger logger)
        {
            _logger = logger;
        }

        public override void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            Log(GetMessageLevel(testMessageLevel), message);
        }

        public override void Clear()
        {
            try
            {
                _logger.Clear();
            }
            catch { }
        }

        public override void Log(MessageLevel messageLevel, string message)
        {
            try
            {
                _logger.Log(messageLevel, FormatMessage(messageLevel, message));
            }
            catch { }
        }
    }
}
