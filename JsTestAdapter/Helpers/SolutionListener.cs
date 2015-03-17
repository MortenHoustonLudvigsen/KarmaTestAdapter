using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;

namespace JsTestAdapter.Helpers
{
    public enum SolutionChangedReason
    {
        Load,
        Unload,
        Open,
        Close
    }

    public class SolutionListenerEventArgs : EventArgs
    {
        public IVsProject Project { get; private set; }
        public SolutionChangedReason ChangedReason { get; private set; }

        public SolutionListenerEventArgs(IVsProject project, SolutionChangedReason reason)
        {
            Project = project;
            ChangedReason = reason;
        }
    }

    public interface ISolutionListener : IDisposable
    {
        void StartListening();
        void StopListening();
        event EventHandler<SolutionListenerEventArgs> ProjectChanged;
        event EventHandler SolutionLoaded;
        event EventHandler SolutionUnloaded;
    }

    [Export(typeof(ISolutionListener))]
    public class SolutionListener : IVsSolutionEvents, ISolutionListener, IDisposable
    {
        private IVsSolution _solution;
        private uint _cookie = VSConstants.VSCOOKIE_NIL;

        [ImportingConstructor]
        public SolutionListener([Import(typeof(SVsServiceProvider))]IServiceProvider serviceProvider)
        {
            ValidateArg.NotNull(serviceProvider, "serviceProvider");
            _solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
        }


        /// <summary>
        /// Fires an event when a project is opened/closed/loaded/unloaded
        /// </summary>
        public event EventHandler<SolutionListenerEventArgs> ProjectChanged;

        public event EventHandler SolutionLoaded;
        public event EventHandler SolutionUnloaded;

        public void StartListening()
        {
            if (_solution != null)
            {
                int hr = _solution.AdviseSolutionEvents(this, out _cookie);
                ErrorHandler.ThrowOnFailure(hr); // do nothing if this fails
            }
        }

        public void StopListening()
        {
            if (_cookie != VSConstants.VSCOOKIE_NIL && _solution != null)
            {
                int hr = _solution.UnadviseSolutionEvents(_cookie);
                ErrorHandler.Succeeded(hr); // do nothing if this fails

                _cookie = VSConstants.VSCOOKIE_NIL;
                _solution = null;
            }
        }

        public void OnSolutionProjectUpdated(IVsProject project, SolutionChangedReason reason)
        {
            if (ProjectChanged != null && project != null)
            {
                ProjectChanged(this, new SolutionListenerEventArgs(project, reason));
            }
        }

        public void OnSolutionLoaded()
        {
            if (SolutionLoaded != null)
            {
                SolutionLoaded(this, new System.EventArgs());
            }
        }

        public void OnSolutionUnloaded()
        {
            if (SolutionUnloaded != null)
            {
                SolutionUnloaded(this, new System.EventArgs());
            }
        }


        /// <summary>
        /// This event is called when a project has been reloaded. This happens when you choose to unload a project 
        /// (often to edit its .proj file) and then reload it.
        /// </summary>
        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            var project = pRealHierarchy as IVsProject;
            OnSolutionProjectUpdated(project, SolutionChangedReason.Load);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This gets called when a project is unloaded
        /// </summary>
        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            OnSolutionProjectUpdated(pRealHierarchy as IVsProject, SolutionChangedReason.Unload);
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            OnSolutionLoaded();
            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved)
        {
            OnSolutionUnloaded();
            return VSConstants.S_OK;
        }

        // Unused events...

        /// <summary>
        /// This gets called when a project is opened
        /// </summary>
        /// <param name="pHierarchy"></param>
        /// <param name="fAdded">0 if alreay part of solution, 1 if it is being added to the solution</param>
        /// <returns></returns>
        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            OnSolutionProjectUpdated(pHierarchy as IVsProject, SolutionChangedReason.Open);
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            OnSolutionProjectUpdated(pHierarchy as IVsProject, SolutionChangedReason.Close);
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        public void Dispose()
        {
            Dispose(true);
            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopListening();
            }
        }
    }
}