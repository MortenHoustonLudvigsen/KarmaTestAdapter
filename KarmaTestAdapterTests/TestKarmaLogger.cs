using KarmaTestAdapter.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapterTests
{
    public class TestKarmaLogger : IKarmaLogger
    {
        public IKarmaLogger Parent { get { return null; } set { } }
        public string Phase { get { return null; } }

        public void Info(string message)
        {
        }

        public void Info(string message, params object[] args)
        {
        }

        public void Warn(string message)
        {
        }

        public void Warn(string message, params object[] args)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string message, params object[] args)
        {
        }

        public void Error(Exception ex, string message = null)
        {
        }

        public void Error(Exception ex, string message = null, params object[] args)
        {
        }

        public string FormatMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel level, string message)
        {
            return "";
        }

        public string FormatMessage(Microsoft.VisualStudio.TestWindow.Extensibility.MessageLevel level, string message)
        {
            return "";
        }

        public bool HasLogger(IKarmaLogger logger)
        {
            return false;
        }

        public bool HasLogger(Microsoft.VisualStudio.TestWindow.Extensibility.ILogger logger)
        {
            return false;
        }

        public bool HasLogger(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.IMessageLogger logger)
        {
            return false;
        }

        public bool HasLogger(string filename)
        {
            return false;
        }

        public bool HasLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger
        {
            return false;
        }

        public void AddLogger(IKarmaLogger logger)
        {
        }

        public void AddLogger(Microsoft.VisualStudio.TestWindow.Extensibility.ILogger logger)
        {
        }

        public void AddLogger(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.IMessageLogger logger)
        {
        }

        public void AddLogger(string filename)
        {
        }

        public void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger) where TLogger : IKarmaLogger
        {
        }

        public void RemoveLogger(IKarmaLogger logger)
        {
        }

        public void RemoveLogger(Microsoft.VisualStudio.TestWindow.Extensibility.ILogger logger)
        {
        }

        public void RemoveLogger(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.IMessageLogger logger)
        {
        }

        public void RemoveLogger(string filename)
        {
        }

        public void RemoveLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger
        {
        }

        public void SendMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel testMessageLevel, string message)
        {
        }

        public void Clear()
        {
        }

        public void Log(Microsoft.VisualStudio.TestWindow.Extensibility.MessageLevel messageLevel, string message)
        {
        }
    }
}
