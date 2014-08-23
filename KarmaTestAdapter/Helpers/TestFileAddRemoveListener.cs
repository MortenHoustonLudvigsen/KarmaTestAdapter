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
    public sealed class TestFileAddRemoveListener : IVsTrackProjectDocumentsEvents2, IDisposable, ITestFileAddRemoveListener
    {
        private readonly IVsTrackProjectDocuments2 _projectDocTracker;
        private uint _cookie = VSConstants.VSCOOKIE_NIL;

        [ImportingConstructor]
        public TestFileAddRemoveListener([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            ValidateArg.NotNull(serviceProvider, "serviceProvider");

            _projectDocTracker = serviceProvider.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
        }

        public event EventHandler<TestFileChangedEventArgs> TestFileChanged;

        public void StartListeningForTestFileChanges()
        {
            if (_projectDocTracker != null)
            {
                int hr = _projectDocTracker.AdviseTrackProjectDocumentsEvents(this, out _cookie);
                ErrorHandler.ThrowOnFailure(hr); // do nothing if this fails
            }
        }

        public void StopListeningForTestFileChanges()
        {
            if (_cookie != VSConstants.VSCOOKIE_NIL && _projectDocTracker != null)
            {
                int hr = _projectDocTracker.UnadviseTrackProjectDocumentsEvents(_cookie);
                ErrorHandler.Succeeded(hr); // do nothing if this fails

                _cookie = VSConstants.VSCOOKIE_NIL;
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
                    if (changedProjects[changeProjIndex] != null && TestFileChanged != null)
                    {
                        TestFileChanged(this, new TestFileChangedEventArgs(changedProjectItems[projItemIndex], reason));
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
                StopListeningForTestFileChanges();
            }
        }
    }
}