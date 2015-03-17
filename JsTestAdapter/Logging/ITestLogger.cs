using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Logging
{
    public interface ITestLogger
    {
        IEnumerable<string> Context { get; }
        void Clear();
        void Log(TestLogLevel level, IEnumerable<string> context, string message);
        string FormatMessage(TestLogLevel level, IEnumerable<string> context, string message);
        bool HasLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : ITestLogger;
        void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger) where TLogger : ITestLogger;
        void RemoveLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : ITestLogger;
    }
}
