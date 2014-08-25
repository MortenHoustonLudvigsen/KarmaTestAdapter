using KarmaTestAdapter.Logging;
using System;

namespace KarmaTestAdapter.Helpers
{
    public interface ITestFilesUpdateWatcher : IDisposable
    {
        event EventHandler<TestFileChangedEventArgs> Changed;
        void AddDirectory(string path);
        void RemoveDirectory(string path);
        void Clear();
        void AddWatch(string path);
        void RemoveWatch(string path);
    }
}