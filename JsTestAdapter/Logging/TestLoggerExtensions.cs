using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JsTestAdapter.Logging
{
    public static class TestLoggerExtensions
    {
        public static TestLogLevel GetTestLogLevel(this TestMessageLevel level)
        {
            switch (level)
            {
                case TestMessageLevel.Informational:
                    return TestLogLevel.Informational;
                case TestMessageLevel.Warning:
                    return TestLogLevel.Warning;
                case TestMessageLevel.Error:
                    return TestLogLevel.Error;
                default:
                    return TestLogLevel.Error;
            }
        }

        public static TestLogLevel GetTestLogLevel(this MessageLevel level)
        {
            switch (level)
            {
                case MessageLevel.Informational:
                    return TestLogLevel.Informational;
                case MessageLevel.Warning:
                    return TestLogLevel.Warning;
                case MessageLevel.Error:
                    return TestLogLevel.Error;
                case MessageLevel.Diagnostic:
                    return TestLogLevel.Diagnostic;
                default:
                    return TestLogLevel.Error;
            }
        }

        public static string LevelText(this TestLogLevel level)
        {
            switch (level)
            {
                case TestLogLevel.Informational:
                    //return "Info";
                    return "";
                case TestLogLevel.Warning:
                    return "Warning";
                case TestLogLevel.Error:
                    return "Error";
                case TestLogLevel.Diagnostic:
                    return "Debug";
                default:
                    return "Error";
            }
        }

        public static string FormatMessage(this ITestLogger logger, TestMessageLevel level, string message)
        {
            return logger.FormatMessage(level.GetTestLogLevel(), logger.Context, message);
        }

        public static string FormatMessage(this ITestLogger logger, MessageLevel level, string message)
        {
            return logger.FormatMessage(level.GetTestLogLevel(), logger.Context, message);
        }

        public static void Info(this ITestLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(TestLogLevel.Informational, logger.Context, message);
            }
        }

        public static void Info(this ITestLogger logger, string message, params object[] args)
        {
            logger.Info(string.Format(message, args));
        }

        public static void Warn(this ITestLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(TestLogLevel.Warning, logger.Context, message);
            }
        }

        public static void Warn(this ITestLogger logger, string message, params object[] args)
        {
            logger.Warn(string.Format(message, args));
        }

        public static void Debug(this ITestLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(TestLogLevel.Diagnostic, logger.Context, message);
            }
        }

        public static void Debug(this ITestLogger logger, string message, params object[] args)
        {
            logger.Debug(string.Format(message, args));
        }

        public static void Error(this ITestLogger logger, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                logger.Log(TestLogLevel.Error, logger.Context, message);
            }
        }

        public static void Error(this ITestLogger logger, string message, params object[] args)
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

        public static void Error(this ITestLogger logger, Exception ex, string message = null)
        {
            logger.Error(ExceptionText(ex, new StringBuilder(), message).ToString());
        }

        public static void Error(this ITestLogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.Error(ex, string.Format(message, args));
        }

        public static bool HasLogger(this ITestLogger logger, ITestLogger childLogger)
        {
            return logger.HasLogger<ITestLogger>(l => l == childLogger);
        }

        public static bool HasLogger(this ITestLogger logger, ILogger childLogger)
        {
            return logger.HasLogger<ExtensibilityTestLogger>(l => l.Logger == childLogger);
        }

        public static bool HasLogger(this ITestLogger logger, IMessageLogger childLogger)
        {
            return logger.HasLogger<TestMessageTestLogger>(l => l.Logger == childLogger);
        }

        public static bool HasLogger(this ITestLogger logger, string filename)
        {
            return logger.HasLogger<FileTestLogger>(l => l.Filename == filename);
        }

        public static void AddLogger(this ITestLogger logger, ITestLogger childLogger)
        {
            if (logger != null)
            {
                logger.AddLogger(l => l == childLogger, () => childLogger);
            }
        }

        public static void AddLogger(this ITestLogger logger, ILogger childLogger)
        {
            if (logger != null)
            {
                logger.AddLogger(l => l.Logger == childLogger, () => new ExtensibilityTestLogger(childLogger));
            }
        }

        public static void AddLogger(this ITestLogger logger, IMessageLogger childLogger)
        {
            if (logger != null)
            {
                logger.AddLogger(l => l.Logger == childLogger, () => new TestMessageTestLogger(childLogger));
            }
        }

        public static void AddLogger(this ITestLogger logger, string filename)
        {
            if (!string.IsNullOrWhiteSpace(filename))
            {
                logger.AddLogger(l => l.Filename == filename, () => new FileTestLogger(filename));
            }
        }

        public static void RemoveLogger(this ITestLogger logger, ITestLogger childLogger)
        {
            logger.RemoveLogger<ITestLogger>(l => l == childLogger);
        }

        public static void RemoveLogger(this ITestLogger logger, ILogger childLogger)
        {
            logger.RemoveLogger<ExtensibilityTestLogger>(l => l.Logger == childLogger);
        }

        public static void RemoveLogger(this ITestLogger logger, IMessageLogger childLogger)
        {
            logger.RemoveLogger<TestMessageTestLogger>(l => l.Logger == childLogger);
        }

        public static void RemoveLogger(this ITestLogger logger, string filename)
        {
            logger.RemoveLogger<FileTestLogger>(l => l.Filename == filename);
        }
    }
}