using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Logging
{
    public abstract class TestLoggerBase : ITestLogger, IDisposable
    {
        public TestLoggerBase()
        {
        }

        protected void AddContext(string context)
        {
            if (!string.IsNullOrWhiteSpace(context))
            {
                _context.Add(context);
            }
        }

        protected void AddContext(IEnumerable<string> context)
        {
            _context.AddRange(context.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private List<string> _context = new List<string>();
        public IEnumerable<string> Context
        {
            get { return _context; }
        }

        public virtual void Clear()
        {
            // Do nothing
        }

        public abstract void Log(TestLogLevel level, IEnumerable<string> context, string message);

        public string FormatMessage(TestLogLevel level, IEnumerable<string> context, string message)
        {
            return FormatMessageInternal(level, context, message);
        }

        private IEnumerable<string> FlattenParts(object p)
        {
            if (p is string)
            {
                var partStr = p as string;
                if (!string.IsNullOrWhiteSpace(partStr))
                {
                    yield return partStr;
                }
            }
            else if (p is IEnumerable<object>)
            {
                var partList = p as IEnumerable<object>;

                foreach (var child in partList.SelectMany(c => FlattenParts(c)))
                {
                    yield return child;
                }
            }
        }

        protected string FormatMessage(IEnumerable<object> parts)
        {
            var message = FlattenParts(parts);

            return string.Format("{0} {1}",
                string.Join(" ", message.Take(message.Count() - 1).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => "[" + p + "]")),
                message.Last()
            );
        }

        protected virtual string FormatMessageInternal(TestLogLevel level, IEnumerable<string> context, string message)
        {
            context = context ?? Enumerable.Empty<string>();
            var parts = new object[]{
                context.FirstOrDefault(),
                level.LevelText(),
                context.Skip(1),
                message
            };
            return FormatMessage(parts);
        }

        public virtual bool HasLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : ITestLogger
        {
            return false;
        }

        public virtual void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger) where TLogger : ITestLogger
        {
            // Do nothing
        }

        public virtual void RemoveLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : ITestLogger
        {
            // Do nothing
        }

        public virtual void Dispose()
        {
        }
    }
}
