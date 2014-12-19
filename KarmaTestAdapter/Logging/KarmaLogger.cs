using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public class KarmaLogger : KarmaLoggerBase
    {
        public static IKarmaLogger Create(ILogger logger = null, IMessageLogger messageLogger = null)
        {
            var karmaLogger = new KarmaLogger();
            karmaLogger.AddLogger(logger);
            karmaLogger.AddLogger(messageLogger);
            return karmaLogger;
        }

        private readonly List<IKarmaLogger> _loggers = new List<IKarmaLogger>();

        public KarmaLogger(params IKarmaLogger[] loggers)
        {
            foreach (var logger in loggers)
            {
                AddLogger(logger);
            }
        }

        public override bool HasLogger<TLogger>(Func<TLogger, bool> predicate)
        {
            if (_loggers.OfType<TLogger>().Any(predicate))
            {
                return true;
            }
            foreach (var logger in _loggers)
            {
                if (logger.HasLogger<TLogger>(predicate))
                {
                    return true;
                }
            }
            return false;
        }

        public override void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger)
        {
            if (!HasLogger(predicate))
            {
                var logger = createLogger();
                logger.Parent = this;
                _loggers.Add(logger);
            }
        }

        public override void RemoveLogger<TLogger>(Func<TLogger, bool> predicate)
        {
            var loggersToRemove = _loggers.OfType<TLogger>().Where(predicate).Cast<IKarmaLogger>().ToList();
            _loggers.RemoveAll(l => loggersToRemove.Contains(l));
            foreach (var logger in loggersToRemove)
            {
                logger.Parent = null;
            }
        }

        public override void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.SendMessage(testMessageLevel, message);
                }
                catch { }
            }
        }

        public override void Clear()
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Clear();
                }
                catch { }
            }
        }

        public override void Log(MessageLevel messageLevel, string message)
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(messageLevel, message);
                }
                catch { }
            }
        }
    }
}
