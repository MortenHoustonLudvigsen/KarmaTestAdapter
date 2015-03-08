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

        public override void Clear()
        {
            try
            {
                File.Delete(Filename);
            }
            catch { }
        }

        public override void Log(KarmaLogLevel level, string phase, string message)
        {
            try
            {
                using (var file = File.AppendText(Filename))
                {
                    file.WriteLine(FormatMessage(level, phase, message));
                }
            }
            catch { }
        }

        protected override string FormatMessageInternal(KarmaLogLevel level, string phase, string message)
        {
            return FormatMessage(
                DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"),
                phase,
                level.LevelText(),
                message
            );
        }
    }
}
