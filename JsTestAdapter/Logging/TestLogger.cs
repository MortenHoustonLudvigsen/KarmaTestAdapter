using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Logging
{
    public class TestLogger : TestLoggerBase
    {
        protected TestLogger()
        {
        }

        public TestLogger(ITestLogger logger, params string[] context)
        {
            AddContext(logger.Context);
            AddContext(context);
            this.AddLogger(logger);
        }

        private readonly List<ITestLogger> _loggers = new List<ITestLogger>();

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
            var loggersToRemove = _loggers.OfType<TLogger>().Where(predicate).Cast<ITestLogger>().ToList();
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

        public override void Log(TestLogLevel level, IEnumerable<string> context, string message)
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
