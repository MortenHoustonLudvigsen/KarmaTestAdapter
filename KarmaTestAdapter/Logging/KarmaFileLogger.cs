using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public class KarmaFileLogger : KarmaLoggerBase
    {
        private string _filename;

        public KarmaFileLogger(string filename)
        {
            _filename = filename;
        }

        public override void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            Log(GetMessageLevel(testMessageLevel), message);
        }

        public override void Clear()
        {
            try
            {
                File.Delete(_filename);
            }
            catch { }
        }

        public override void Log(MessageLevel messageLevel, string message)
        {
            try
            {
                using (var file = File.AppendText(_filename))
                {
                    file.WriteLine(FormatMessage(messageLevel, message));
                }
            }
            catch { }
        }

        protected override string FormatMessage(MessageLevel level, string message)
        {
            return string.Format("[{0:dd.MM.yyyy HH:mm:ss}] [{1}] {2}", DateTime.Now, LevelText(level), message);
        }
    }
}
