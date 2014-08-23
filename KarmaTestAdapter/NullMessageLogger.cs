using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public class NullMessageLogger : IMessageLogger, ILogger
    {
        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
        }

        public void Clear()
        {
        }

        public void Log(MessageLevel messageLevel, string message)
        {
        }
    }
}
