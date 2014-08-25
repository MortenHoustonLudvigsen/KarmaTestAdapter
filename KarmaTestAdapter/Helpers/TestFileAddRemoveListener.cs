using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using KarmaTestAdapter;

namespace KarmaTestAdapter.Helpers
{
    [Export(typeof(ITestFileAddRemoveListener))]
    public sealed class TestFileAddRemoveListener : IVsTrackProjectDocumentsEvents2, IVsRunningDocTableEvents, IDisposable, ITestFileAddRemoveListener
    {
        private IVsRunningDocumentTable _documentTable;
        private uint _documentTableItemId;

        private readonly IVsTrackProjectDocuments2 _projectDocTracker;
        private uint _cookie = VSConstants.VSCOOKIE_NIL;

        [ImportingConstructor]
        public TestFileAddRemoveListener([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            ValidateArg.NotNull(serviceProvider, "serviceProvider");

            _projectDocTracker = serviceProvider.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
            _documentTable = Package.GetGlobalService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
        }

        public event EventHandler<TestFileChangedEventArgs> Changed;
        void OnChanged(object sender, TestFileChangedEventArgs e)
        {
            if (Changed != null)
            {
                Changed(sender, e);
            }
        }

        public void StartListening()
        {
            if (_projectDocTracker != null)
            {
                int hr = _projectDocTracker.AdviseTrackProjectDocumentsEvents(this, out _cookie);
                ErrorHandler.ThrowOnFailure(hr); // do nothing if this fails
            }
            if (_documentTable != null)
            {
                int hr = _documentTable.AdviseRunningDocTableEvents(this, out _documentTableItemId);
                ErrorHandler.ThrowOnFailure(hr);
            }
        }

        public void StopListening()
        {
            if (_cookie != VSConstants.VSCOOKIE_NIL && _projectDocTracker != null)
            {
                int hr = _projectDocTracker.UnadviseTrackProjectDocumentsEvents(_cookie);
                ErrorHandler.Succeeded(hr); // do nothing if this fails

                _cookie = VSConstants.VSCOOKIE_NIL;
            }
            if (_documentTable != null)
            {
                int hr = _documentTable.UnadviseRunningDocTableEvents(_documentTableItemId);
                ErrorHandler.Succeeded(hr);
            }
        }

        private int OnNotifyTestFileAddRemove(int changedProjectCount,
                                              IVsProject[] changedProjects,
                                              string[] changedProjectItems,
                                              int[] rgFirstIndices,
                                              TestFileChangedReason reason)
        {
            // The way these parameters work is:
            // rgFirstIndices contains a list of the starting index into the changeProjectItems array for each project listed in the changedProjects list
            // Example: if you get two projects, then rgFirstIndices should have two elements, the first element is probably zero since rgFirstIndices would start at zero.
            // Then item two in the rgFirstIndices array is where in the changeProjectItems list that the second project's changed items reside.
            int projItemIndex = 0;
            for (int changeProjIndex = 0; changeProjIndex < changedProjectCount; changeProjIndex++)
            {
                int endProjectIndex = ((changeProjIndex + 1) == changedProjectCount) ? changedProjectItems.Length : rgFirstIndices[changeProjIndex + 1];

                for (; projItemIndex < endProjectIndex; projItemIndex++)
                {
                    if (changedProjects[changeProjIndex] != null)
                    {
                        OnChanged(this, new TestFileChangedEventArgs(changedProjectItems[projItemIndex], reason));
                    }

                }
            }
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterAddFilesEx(int cProjects,
                                                              int cFiles,
                                                              IVsProject[] rgpProjects,
                                                              int[] rgFirstIndices,
                                                              string[] rgpszMkDocuments,
                                                              VSADDFILEFLAGS[] rgFlags)
        {
            return OnNotifyTestFileAddRemove(cProjects, rgpProjects, rgpszMkDocuments, rgFirstIndices, TestFileChangedReason.Added);
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRemoveFiles(int cProjects,
                                                               int cFiles,
                                                               IVsProject[] rgpProjects,
                                                               int[] rgFirstIndices,
                                                               string[] rgpszMkDocuments,
                                                               VSREMOVEFILEFLAGS[] rgFlags)
        {
            return OnNotifyTestFileAddRemove(cProjects, rgpProjects, rgpszMkDocuments, rgFirstIndices, TestFileChangedReason.Removed);
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRenameFiles(int cProjects,
                                                               int cFiles,
                                                               IVsProject[] rgpProjects,
                                                               int[] rgFirstIndices,
                                                               string[] rgszMkOldNames,
                                                               string[] rgszMkNewNames,
                                                               VSRENAMEFILEFLAGS[] rgFlags)
        {
            OnNotifyTestFileAddRemove(cProjects, rgpProjects, rgszMkOldNames, rgFirstIndices, TestFileChangedReason.Removed);
            return OnNotifyTestFileAddRemove(cProjects, rgpProjects, rgszMkNewNames, rgFirstIndices, TestFileChangedReason.Added);
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterAddDirectoriesEx(int cProjects,
                                                                    int cDirectories,
                                                                    IVsProject[] rgpProjects,
                                                                    int[] rgFirstIndices,
                                                                    string[] rgpszMkDocuments,
                                                                    VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterRemoveDirectories(int cProjects,
                                                                     int cDirectories,
                                                                     IVsProject[] rgpProjects,
                                                                     int[] rgFirstIndices,
                                                                     string[] rgpszMkDocuments,
                                                                     VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }


        int IVsTrackProjectDocumentsEvents2.OnAfterRenameDirectories(int cProjects,
                                                                     int cDirs,
                                                                     IVsProject[] rgpProjects,
                                                                     int[] rgFirstIndices,
                                                                     string[] rgszMkOldNames,
                                                                     string[] rgszMkNewNames,
                                                                     VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnAfterSccStatusChanged(int cProjects,
                                                                    int cFiles,
                                                                    IVsProject[] rgpProjects,
                                                                    int[] rgFirstIndices,
                                                                    string[] rgpszMkDocuments,
                                                                    uint[] rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryAddDirectories(IVsProject pProject,
                                                                  int cDirectories,
                                                                  string[] rgpszMkDocuments,
                                                                  VSQUERYADDDIRECTORYFLAGS[] rgFlags,
                                                                  VSQUERYADDDIRECTORYRESULTS[] pSummaryResult,
                                                                  VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryAddFiles(IVsProject pProject,
                                                            int cFiles,
                                                            string[] rgpszMkDocuments,
                                                            VSQUERYADDFILEFLAGS[] rgFlags,
                                                            VSQUERYADDFILERESULTS[] pSummaryResult,
                                                            VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRemoveDirectories(IVsProject pProject,
                                                                     int cDirectories,
                                                                     string[] rgpszMkDocuments,
                                                                     VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags,
                                                                     VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult,
                                                                     VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRemoveFiles(IVsProject pProject,
                                                               int cFiles,
                                                               string[] rgpszMkDocuments,
                                                               VSQUERYREMOVEFILEFLAGS[] rgFlags,
                                                               VSQUERYREMOVEFILERESULTS[] pSummaryResult,
                                                               VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRenameDirectories(IVsProject pProject,
                                                                     int cDirs,
                                                                     string[] rgszMkOldNames,
                                                                     string[] rgszMkNewNames,
                                                                     VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags,
                                                                     VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult,
                                                                     VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        int IVsTrackProjectDocumentsEvents2.OnQueryRenameFiles(IVsProject pProject,
                                                               int cFiles,
                                                               string[] rgszMkOldNames,
                                                               string[] rgszMkNewNames,
                                                               VSQUERYRENAMEFILEFLAGS[] rgFlags,
                                                               VSQUERYRENAMEFILERESULTS[] pSummaryResult,
                                                               VSQUERYRENAMEFILERESULTS[] rgResults)
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

        #region IVsRunningDocTableEvents

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie)
        {
            uint flags;
            uint readingLocks;
            uint edittingLocks;
            string name;
            IVsHierarchy hierarchy;
            uint documentId;
            IntPtr documentData;

            _documentTable.GetDocumentInfo(docCookie, out flags, out readingLocks, out edittingLocks, out name, out hierarchy, out documentId, out documentData);
            OnChanged(this, new TestFileChangedEventArgs(name, TestFileChangedReason.Saved));
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame pFrame)
        {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRDTLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}