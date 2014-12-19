using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Logging
{
    public interface IKarmaLogger : IMessageLogger, ILogger
    {
        IKarmaLogger Parent { get; set; }
        string Phase { get; }
        void Info(string message);
        void Info(string message, params object[] args);
        void Warn(string message);
        void Warn(string message, params object[] args);
        void Error(string message);
        void Error(string message, params object[] args);
        void Error(Exception ex, string message = null);
        void Error(Exception ex, string message = null, params object[] args);
        string FormatMessage(TestMessageLevel level, string message);
        string FormatMessage(MessageLevel level, string message);
        bool HasLogger(IKarmaLogger logger);
        bool HasLogger(ILogger logger);
        bool HasLogger(IMessageLogger logger);
        bool HasLogger(string filename);
        bool HasLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger;
        void AddLogger(IKarmaLogger logger);
        void AddLogger(ILogger logger);
        void AddLogger(IMessageLogger logger);
        void AddLogger(string filename);
        void AddLogger<TLogger>(Func<TLogger, bool> predicate, Func<TLogger> createLogger) where TLogger : IKarmaLogger;
        void RemoveLogger(IKarmaLogger logger);
        void RemoveLogger(ILogger logger);
        void RemoveLogger(IMessageLogger logger);
        void RemoveLogger(string filename);
        void RemoveLogger<TLogger>(Func<TLogger, bool> predicate) where TLogger : IKarmaLogger;
    }
}
