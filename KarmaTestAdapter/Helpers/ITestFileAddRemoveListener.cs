using System;

namespace KarmaTestAdapter.Helpers
{
    public interface ITestFileAddRemoveListener: IDisposable
    {
        event EventHandler<TestFileChangedEventArgs> Changed;
        void StartListening();
        void StopListening();
    }
}