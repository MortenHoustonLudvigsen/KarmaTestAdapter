using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KarmaTestAdapter.Logging
{
    public static class Extensions
    {
        public static KarmaLogLevel GetKarmaLogLevel(this TestMessageLevel level)
        {
            switch (level)
            {
                case TestMessageLevel.Informational:
                    return KarmaLogLevel.Informational;
                case TestMessageLevel.Warning:
                    return KarmaLogLevel.Warning;
                case TestMessageLevel.Error:
                    return KarmaLogLevel.Error;
                default:
                    return KarmaLogLevel.Error;
            }
        }

        public static KarmaLogLevel GetKarmaLogLevel(this MessageLevel level)
        {
            switch (level)
            {
                case MessageLevel.Informational:
                    return KarmaLogLevel.Informational;
                case MessageLevel.Warning:
                    return KarmaLogLevel.Warning;
                case MessageLevel.Error:
                    return KarmaLogLevel.Error;
                case MessageLevel.Diagnostic:
                    return KarmaLogLevel.Diagnostic;
                default:
                    return KarmaLogLevel.Error;
            }
        }

        public static string LevelText(this KarmaLogLevel level)
        {
            switch (level)
            {
                case KarmaLogLevel.Informational:
                    //return "Info";
                    return "";
                case KarmaLogLevel.Warning:
                    return "Warning";
                case KarmaLogLevel.Error:
                    return "Error";
                case KarmaLogLevel.Diagnostic:
                    return "Debug";
                default:
                    return "Error";
            }
        }

        public static string FormatMessage(this IKarmaLogger logger, TestMessageLevel level, string message)
        {
            return logger.FormatMessage(level.GetKarmaLogLevel(), logger.Context, message);
        }

        public static string FormatMessage(this IKarmaLogger logger, MessageLevel level, string message)
        {
            return logger.FormatMessage(level.GetKarmaLogLevel(), logger.Context, message);
        }

        public static void Info(this IKarmaLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(KarmaLogLevel.Informational, logger.Context, message);
            }
        }

        public static void Info(this IKarmaLogger logger, string message, params object[] args)
        {
            logger.Info(string.Format(message, args));
        }

        public static void Warn(this IKarmaLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(KarmaLogLevel.Warning, logger.Context, message);
            }
        }

        public static void Warn(this IKarmaLogger logger, string message, params object[] args)
        {
            logger.Warn(string.Format(message, args));
        }

        public static void Debug(this IKarmaLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(KarmaLogLevel.Diagnostic, logger.Context, message);
            }
        }

        public static void Debug(this IKarmaLogger logger, string message, params object[] args)
        {
            logger.Debug(string.Format(message, args));
        }

        public static void Error(this IKarmaLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(KarmaLogLevel.Error, logger.Context, message);
            }
        }

        public static void Error(this IKarmaLogger logger, string message, params object[] args)
        {
            logger.Error(string.Format(message, args));
        }

        private static StringBuilder ExceptionText(Exception ex, StringBuilder text, string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                text.AppendLine(message);
            }
            if (ex != null)
            {
                text.AppendLine(ex.Message);
                if (ex is AggregateException)
                {
                    AggregateExceptionText(ex as AggregateException, text);
                }
                else
                {
                    text.AppendLine(ex.StackTrace);
                    ExceptionText(ex.InnerException, text);
                }
            }
            return text;
        }

        private static StringBuilder AggregateExceptionText(AggregateException ex, StringBuilder text)
        {
            foreach (var exception in ex.InnerExceptions)
            {
                ExceptionText(exception, text);
            }
            return text;
        }

        public static void Error(this IKarmaLogger logger, Exception ex, string message = null)
        {
            logger.Error(ExceptionText(ex, new StringBuilder(), message).ToString());
        }

        public static void Error(this IKarmaLogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.Error(ex, string.Format(message, args));
        }

        public static bool HasLogger(this IKarmaLogger logger, IKarmaLogger childLogger)
        {
            return logger.HasLogger<IKarmaLogger>(l => l == childLogger);
        }

        public static bool HasLogger(this IKarmaLogger logger, ILogger childLogger)
        {
            return logger.HasLogger<KarmaExtensibilityLogger>(l => l.Logger == childLogger);
        }

        public static bool HasLogger(this IKarmaLogger logger, IMessageLogger childLogger)
        {
            return logger.HasLogger<KarmaTestMessageLogger>(l => l.Logger == childLogger);
        }

        public static bool HasLogger(this IKarmaLogger logger, string filename)
        {
            return logger.HasLogger<KarmaFileLogger>(l => l.Filename == filename);
        }

        public static void AddLogger(this IKarmaLogger logger, IKarmaLogger childLogger)
        {
            if (logger != null)
            {
                logger.AddLogger(l => l == childLogger, () => childLogger);
            }
        }

        public static void AddLogger(this IKarmaLogger logger, ILogger childLogger)
        {
            if (logger != null)
            {
                logger.AddLogger(l => l.Logger == childLogger, () => new KarmaExtensibilityLogger(childLogger));
            }
        }

        public static void AddLogger(this IKarmaLogger logger, IMessageLogger childLogger)
        {
            if (logger != null)
            {
                logger.AddLogger(l => l.Logger == childLogger, () => new KarmaTestMessageLogger(childLogger));
            }
        }

        public static void AddLogger(this IKarmaLogger logger, string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                logger.AddLogger(l => l.Filename == filename, () => new KarmaFileLogger(filename));
            }
        }

        public static void RemoveLogger(this IKarmaLogger logger, IKarmaLogger childLogger)
        {
            logger.RemoveLogger<IKarmaLogger>(l => l == childLogger);
        }

        public static void RemoveLogger(this IKarmaLogger logger, ILogger childLogger)
        {
            logger.RemoveLogger<KarmaExtensibilityLogger>(l => l.Logger == childLogger);
        }

        public static void RemoveLogger(this IKarmaLogger logger, IMessageLogger childLogger)
        {
            logger.RemoveLogger<KarmaTestMessageLogger>(l => l.Logger == childLogger);
        }

        public static void RemoveLogger(this IKarmaLogger logger, string filename)
        {
            logger.RemoveLogger<KarmaFileLogger>(l => l.Filename == filename);
        }
    }
}