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
        public string Filename { get; private set; }

        public KarmaFileLogger(string filename)
        {
            Filename = filename;
        }

        public override void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            Log(GetMessageLevel(testMessageLevel), message);
        }

        public override void Clear()
        {
            try
            {
                File.Delete(Filename);
            }
            catch { }
        }

        public override void Log(MessageLevel messageLevel, string message)
        {
            try
            {
                using (var file = File.AppendText(Filename))
                {
                    file.WriteLine(FormatMessage(messageLevel, message));
                }
            }
            catch { }
        }

        protected override string FormatMessageInternal(MessageLevel level, string message)
        {
            return FormatMessage(
                DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"),
                Phase,
                LevelText(level),
                message
            );
            return string.Format("[karma] [{0:dd.MM.yyyy HH:mm:ss} {1}] {2}", DateTime.Now, LevelText(level), message);
        }
    }
}
