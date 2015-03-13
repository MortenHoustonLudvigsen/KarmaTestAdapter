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
    public class KarmaLogger : KarmaLoggerBase
    {
        private readonly List<IKarmaLogger> _loggers = new List<IKarmaLogger>();

        public KarmaLogger(ILogger logger, bool newGlobalLog = false)
            : this(logger, null, newGlobalLog)
        {
        }

        public KarmaLogger(IMessageLogger messageLogger, bool newGlobalLog = false)
            : this(null, messageLogger, newGlobalLog)
        {
        }

        public KarmaLogger(IKarmaLogger karmaLogger, params string[] context)
        {
            AddContext(karmaLogger.Context);
            AddContext(context);
            this.AddLogger(karmaLogger);
        }

        private KarmaLogger(ILogger logger, IMessageLogger messageLogger, bool newGlobalLog)
        {
            if (Globals.Debug)
            {
                if (!Directory.Exists(Globals.GlobalLogDir))
                {
                    Directory.CreateDirectory(Globals.GlobalLogDir);
                }
                if (newGlobalLog && File.Exists(Globals.GlobalLogFile))
                {
                    File.Delete(Globals.GlobalLogFile);
                }
                this.Info("Logging to {0}", Globals.GlobalLogFile);
                this.AddLogger(Globals.GlobalLogFile);
            }
            this.AddLogger(logger);
            this.AddLogger(messageLogger);
        }

        public override bool HasLogger<TLogger>(Func<TLogger, bool> predicate)
        {
            if (_loggers.OfType<TLogger>().Any(predicate))
            {
                return true;
            }
            foreach (var logger in _loggers)
            {
                if (logger.HasLogger(predicate))
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
                _loggers.Add(createLogger());
            }
        }

        public override void RemoveLogger<TLogger>(Func<TLogger, bool> predicate)
        {
            var loggersToRemove = _loggers.OfType<TLogger>().Where(predicate).Cast<IKarmaLogger>().ToList();
            _loggers.RemoveAll(l => loggersToRemove.Contains(l));
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

        public override void Log(KarmaLogLevel level, IEnumerable<string> context, string message)
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(level, context, message);
                }
                catch { }
            }
        }
    }
}
