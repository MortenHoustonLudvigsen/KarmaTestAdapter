using Microsoft.VisualStudio.Shell.Interop;

namespace KarmaTestAdapter.Helpers
{
    public enum SolutionChangedReason
    {
        None,
        Load,
        Unload,
    }

    public class SolutionListenerEventArgs : System.EventArgs
    {
        public IVsProject Project { get; private set; }
        public SolutionChangedReason ChangedReason { get; private set; }

        public SolutionListenerEventArgs(IVsProject project, SolutionChangedReason reason)
        {
            Project = project;
            ChangedReason = reason;
        }
    }
}