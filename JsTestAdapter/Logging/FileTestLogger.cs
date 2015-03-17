using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Logging
{
    public class FileTestLogger : TestLoggerBase
    {
        public string Filename { get; private set; }

        public FileTestLogger(string filename)
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

        public override void Log(TestLogLevel level, IEnumerable<string> context, string message)
        {
            try
            {
                using (var file = File.AppendText(Filename))
                {
                    file.WriteLine(FormatMessage(level, context, message));
                }
            }
            catch { }
        }

        protected override string FormatMessageInternal(TestLogLevel level, IEnumerable<string> context, string message)
        {
            context = context ?? Enumerable.Empty<string>();
            var parts = new object[]{
                DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"),
                context.FirstOrDefault(),
                level.LevelText(),
                context.Skip(1),
                message
            };
            return FormatMessage(parts);
        }
    }
}
