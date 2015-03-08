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

        public KarmaLogger(ILogger logger, string phase, bool newGlobalLog = false)
            : this(logger, null, phase, newGlobalLog)
        {
        }

        public KarmaLogger(IMessageLogger messageLogger, string phase, bool newGlobalLog = false)
            : this(null, messageLogger, phase, newGlobalLog)
        {
        }

        public KarmaLogger(IKarmaLogger karmaLogger, string phase)
        {
            _phase = phase;
            this.AddLogger(karmaLogger);
        }

        private KarmaLogger(ILogger logger, IMessageLogger messageLogger, string phase, bool newGlobalLog)
        {
            _phase = phase;
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

        private string _phase;
        public override string Phase
        {
            get { return _phase ?? base.Phase; }
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

        public override void Log(KarmaLogLevel level, string phase, string message)
        {
            foreach (var logger in _loggers)
            {
                try
                {
                    logger.Log(level, phase, message);
                }
                catch { }
            }
        }
    }
}
