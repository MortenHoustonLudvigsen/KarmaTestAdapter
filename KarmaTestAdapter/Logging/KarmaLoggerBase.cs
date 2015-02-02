using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public abstract class KarmaLoggerBase : IKarmaLogger, IDisposable
    {
        private IKarmaLogger _parent = null;
        public IKarmaLogger Parent
        {
            get { return _parent; }
            set
            {
                if (value != _parent)
                {
                    if (_parent != null)
                    {
                        _parent.RemoveLogger(this);
                    }
                    _parent = value;
                }
            }
        }

        public virtual string Phase
        {
            get { return Parent == null ? null : Parent.Phase; }
        }

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

        protected StringBuilder ExceptionText(Exception ex, StringBuilder text, string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                text.AppendLine(message);
            }
            text.AppendLine(ex.Message);
            text.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                ExceptionText(ex.InnerException, text);
            }
            return text;
        }

        public virtual void Error(Exception ex, string message = null)
        {
            Error(ExceptionText(ex, new StringBuilder(), message).ToString());
        }

        public virtual void Error(Exception ex, string message = null, params object[] args)
        {
            Error(ex, string.Format(message, args));
        }

        public virtual bool HasLogger(IKarmaLogger logger)
        {
            return HasLogger<IKarmaLogger>(l => l == logger);
        }

        public virtual bool HasLogger(ILogger logger)
        {
            return HasLogger<KarmaExtensibilityLogger>(l => l.Logger == logger);
        }

        public virtual bool HasLogger(IMessageLogger logger)
        {
            return HasLogger<KarmaTestMessageLogger>(l => l.Logger == logger);
        }

        public virtual bool HasLogger(string filename)
        {
            return HasLogger<KarmaFileLogger>(l => l.Filename == filename);
        }

        public virtual bool HasLogger<TLogger>(Func<TLogger, bool> predicate)
            where TLogger : IKarmaLogger
        {
            return false;
        }

        public virtual void AddLogger(IKarmaLogger logger)
        {
            if (logger != null)
            {
                AddLogger<IKarmaLogger>(l => l == logger, () => logger);
            }
        }

        public virtual void AddLogger(ILogger logger)
        {
            if (logger != null)
            {
                AddLogger<KarmaExtensibilityLogger>(l => l.Logger == logger, () => new KarmaExtensibilityLogger(logger));
            }
        }

        public virtual void AddLogger(IMessageLogger logger)
        {
            if (logger != null)
            {
                AddLogger<KarmaTestMessageLogger>(l => l.Logger == logger, () => new KarmaTestMessageLogger(logger));
            }
        }

        public virtual void AddLogger(string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                AddLogger<KarmaFileLogger>(l => l.Filename == filename, () => new KarmaFileLogger(filename));
            }
        }

        public virtual void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger)
            where TLogger : IKarmaLogger
        {
            // Do nothing
        }

        public virtual void RemoveLogger(IKarmaLogger logger)
        {
            RemoveLogger<IKarmaLogger>(l => l == logger);
        }

        public virtual void RemoveLogger(ILogger logger)
        {
            RemoveLogger<KarmaExtensibilityLogger>(l => l.Logger == logger);
        }

        public virtual void RemoveLogger(IMessageLogger logger)
        {
            RemoveLogger<KarmaTestMessageLogger>(l => l.Logger == logger);
        }

        public virtual void RemoveLogger(string filename)
        {
            RemoveLogger<KarmaFileLogger>(l => l.Filename == filename);
        }

        public virtual void RemoveLogger<TLogger>(Func<TLogger, bool> predicate)
            where TLogger : IKarmaLogger
        {
            // Do nothing
        }

        public string FormatMessage(TestMessageLevel level, string message)
        {
            return FormatMessage(GetMessageLevel(level), message);
        }

        public string FormatMessage(MessageLevel level, string message)
        {
            return FormatMessageInternal(level, message);
        }

        protected string FormatMessage(params string[] parts)
        {
            return string.Format("{0} {1}",
                string.Join(" ", parts.Take(parts.Length - 1).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => "[" + p + "]")),
                parts.Last()
            );
        }

        protected virtual string FormatMessageInternal(MessageLevel level, string message)
        {
            return FormatMessage(
                "karma",
                //DateTime.Now.ToString("HH:mm:ss"),
                Phase,
                LevelText(level),
                message
            );
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
                    //return "Info";
                    return "";
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

        public virtual void Dispose()
        {
        }
    }
}
