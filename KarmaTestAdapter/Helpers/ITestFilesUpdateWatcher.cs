using KarmaTestAdapter.Logging;
using System;

namespace KarmaTestAdapter.Helpers
{
    public interface ITestFilesUpdateWatcher
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;
        void AddWatch(string path);
        void RemoveWatch(string path);
    }
}