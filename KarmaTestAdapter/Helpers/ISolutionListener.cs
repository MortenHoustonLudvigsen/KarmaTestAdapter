using System;

namespace KarmaTestAdapter.Helpers
{
    public interface ISolutionListener :  IDisposable
    {
        /// <summary>
        /// Fires an event when a project is opened/closed/loaded/unloaded
        /// </summary>
        event EventHandler<SolutionListenerEventArgs> ProjectChanged;

        void StartListening();
        void StopListening();
        event EventHandler SolutionUnloaded;
    }
}