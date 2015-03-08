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
        private static ulong _nextId = 1;

        public KarmaLoggerBase()
        {
            Id = _nextId++;
        }

        public ulong Id { get; private set; }

        //private IKarmaLogger _parent = null;
        //public IKarmaLogger Parent
        //{
        //    get { return _parent; }
        //    set
        //    {
        //        if (value != _parent)
        //        {
        //            if (_parent != null)
        //            {
        //                _parent.RemoveLogger(this);
        //            }
        //            _parent = value;
        //        }
        //    }
        //}

        public virtual string Phase
        {
            get { return null; }
        }

        public virtual void Clear()
        {
            // Do nothing
        }

        public abstract void Log(KarmaLogLevel level, string phase, string message);

        public string FormatMessage(KarmaLogLevel level, string phase, string message)
        {
            return FormatMessageInternal(level, phase, message);
        }

        protected string FormatMessage(params string[] parts)
        {
            return string.Format("{0} {1}",
                string.Join(" ", parts.Take(parts.Length - 1).Where(p => !string.IsNullOrWhiteSpace(p)).Select(p => "[" + p + "]")),
                parts.Last()
            );
        }

        protected virtual string FormatMessageInternal(KarmaLogLevel level, string phase, string message)
        {
            return FormatMessage(
                "karma",
                //DateTime.Now.ToString("HH:mm:ss"),
                phase,
                level.LevelText(),
                message
            );
        }

        public virtual bool HasLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger
        {
            return false;
        }

        public virtual void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger) where TLogger : IKarmaLogger
        {
            // Do nothing
        }

        public virtual void RemoveLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger
        {
            // Do nothing
        }

        public virtual void Dispose()
        {
        }

        public override string ToString()
        {
            return string.Format("{1}({0})", Id, GetType().Name);
        }
    }
}
