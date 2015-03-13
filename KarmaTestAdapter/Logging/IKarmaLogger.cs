using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public interface IKarmaLogger
    {
        IEnumerable<string> Context { get; }
        void Clear();
        void Log(KarmaLogLevel level, IEnumerable<string> context, string message);
        string FormatMessage(KarmaLogLevel level, IEnumerable<string> context, string message);
        bool HasLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger;
        void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger) where TLogger : IKarmaLogger;
        void RemoveLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger;
    }
}
