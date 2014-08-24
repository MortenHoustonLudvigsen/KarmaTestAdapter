using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public abstract class KarmaLoggerBase : IKarmaLogger
    {
        public abstract void SendMessage(Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging.TestMessageLevel testMessageLevel, string message);
        public abstract void Clear();
        public abstract void Log(Microsoft.VisualStudio.TestWindow.Extensibility.MessageLevel messageLevel, string message);

        public virtual void Info(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Log(MessageLevel.Informational, message);
            }
        }

        public virtual void Info(string message, params object[] args)
        {
            Info(string.Format(message, args));
        }

        public virtual void Warn(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Log(MessageLevel.Warning, message);
            }
        }

        public virtual void Warn(string message, params object[] args)
        {
            Warn(string.Format(message, args));
        }

        public virtual void Error(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Log(MessageLevel.Error, message);
            }
        }

        public virtual void Error(string message, params object[] args)
        {
            Error(string.Format(message, args));
        }

        public virtual void Error(Exception ex, string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Error("{3}{1}{0}{1}Stack trace:{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace, message);
            }
            else
            {
                Error("{0}{1}Stack trace:{1}{2}", ex.Message, Environment.NewLine, ex.StackTrace);
            }
        }

        public virtual void Error(Exception ex, string message = null, params object[] args)
        {
            Error(ex, string.Format(message, args));
        }

        protected string FormatMessage(TestMessageLevel level, string message)
        {
            return FormatMessage(GetMessageLevel(level), message);
        }

        protected virtual string FormatMessage(MessageLevel level, string message)
        {
            return string.Format("[karma] [{0:dd.MM.yyyy HH:mm:ss}] [{1}] {2}", DateTime.Now, LevelText(level), message);
        }

        protected string LevelText(TestMessageLevel level)
        {
            return LevelText(GetMessageLevel(level));
        }

        protected string LevelText(MessageLevel level)
        {
            switch (level)
            {
                case MessageLevel.Informational:
                    return "Info";
                case MessageLevel.Warning:
                    return "Warning";
                case MessageLevel.Error:
                    return "Error";
                case MessageLevel.Diagnostic:
                    return "Debug";
                default:
                    return "Error";
            }
        }

        protected MessageLevel GetMessageLevel(TestMessageLevel level)
        {
            switch (level)
            {
                case TestMessageLevel.Informational:
                    return MessageLevel.Informational;
                case TestMessageLevel.Warning:
                    return MessageLevel.Warning;
                case TestMessageLevel.Error:
                    return MessageLevel.Error;
                default:
                    return MessageLevel.Error;
            }
        }

        protected TestMessageLevel GetTestMessageLevel(MessageLevel level)
        {
            switch (level)
            {
                case MessageLevel.Informational:
                    return TestMessageLevel.Informational;
                case MessageLevel.Warning:
                    return TestMessageLevel.Warning;
                case MessageLevel.Error:
                    return TestMessageLevel.Error;
                default:
                    return TestMessageLevel.Error;
            }
        }
    }
}
