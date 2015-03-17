using JsTestAdapter.Logging;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.Helpers
{
    public class ProjectFileAddedEventArgs : EventArgs
    {
        public IVsProject Project { get; private set; }
        public string File { get; private set; }

        public ProjectFileAddedEventArgs(IVsProject project, string file)
        {
            Project = project;
            File = file;
        }
    }

    public class ProjectFileRemovedEventArgs : EventArgs
    {
        public IVsProject Project { get; private set; }
        public string File { get; private set; }

        public ProjectFileRemovedEventArgs(IVsProject project, string file)
        {
            Project = project;
            File = file;
        }
    }

    public class ProjectFileRenamedEventArgs : EventArgs
    {
        public IVsProject Project { get; private set; }
        public string OldFile { get; private set; }
        public string NewFile { get; private set; }

        public ProjectFileRenamedEventArgs(IVsProject project, string oldFile, string newFile)
        {
            Project = project;
            OldFile = oldFile;
            NewFile = newFile;
        }
    }

    public interface IProjectListener : IDisposable
    {
        void StartListening();
        void StopListening();
        event EventHandler<ProjectFileAddedEventArgs> FileAdded;
        event EventHandler<ProjectFileRemovedEventArgs> FileRemoved;
        event EventHandler<ProjectFileRenamedEventArgs> FileRenamed;
    }

    [Export(typeof(IProjectListener))]
    public class ProjectListener : IVsTrackProjectDocumentsEvents2, IProjectListener, IDisposable
    {
        private readonly IVsTrackProjectDocuments2 _projectDocTracker;
        private uint _cookie = VSConstants.VSCOOKIE_NIL;
        private ITestLogger Logger;


        [ImportingConstructor]
        public ProjectListener([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider, ITestLogger logger)
        {
            ValidateArg.NotNull(serviceProvider, "serviceProvider");
            _projectDocTracker = serviceProvider.GetService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
            Logger = new TestLogger(logger, "ProjectListener");
        }

        public event EventHandler<ProjectFileAddedEventArgs> FileAdded;
        void OnFileAdded(object sender, ProjectFileAddedEventArgs e)
        {
            if (FileAdded != null)
            {
                FileAdded(sender, e);
            }
        }

        public event EventHandler<ProjectFileRemovedEventArgs> FileRemoved;
        void OnFileRemoved(object sender, ProjectFileRemovedEventArgs e)
        {
            if (FileRemoved != null)
            {
                FileRemoved(sender, e);
            }
        }

        public event EventHandler<ProjectFileRenamedEventArgs> FileRenamed;
        void OnFileRenamed(object sender, ProjectFileRenamedEventArgs e)
        {
            if (FileRenamed != null)
            {
                FileRenamed(sender, e);
            }
        }

        public void StartListening()
        {
            if (_projectDocTracker != null)
            {
                int hr = _projectDocTracker.AdviseTrackProjectDocumentsEvents(this, out _cookie);
                ErrorHandler.ThrowOnFailure(hr); // do nothing if this fails
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
        }

        private class Event
        {
            public IVsProject Project { get; set; }
            public int ItemIndex { get; set; }
        }

        private IEnumerable<Event> GetEvents(int changedProjectCount, IVsProject[] changedProjects, int[] rgFirstIndices, int maxIndex)
        {
            foreach (var project in changedProjects.Take(changedProjectCount).Select((p, i) => new { Project = p, Index = i }))
            {
                if (project.Project != null)
                {
                    var firstIndex = rgFirstIndices[project.Index];
                    var nextIndex = project.Index < rgFirstIndices.Length - 1 ? rgFirstIndices[project.Index + 1] : maxIndex;

                    for (var itemIndex = firstIndex; itemIndex < nextIndex; itemIndex++)
                    {
                        yield return new Event { Project = project.Project, ItemIndex = itemIndex };
                    }
                }
            }
        }

        public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
        {
            foreach (var evt in GetEvents(cProjects, rgpProjects, rgFirstIndices, rgpszMkDocuments.Length))
            {
                OnFileAdded(this, new ProjectFileAddedEventArgs(evt.Project, rgpszMkDocuments[evt.ItemIndex]));
            }
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
        {
            foreach (var evt in GetEvents(cProjects, rgpProjects, rgFirstIndices, rgpszMkDocuments.Length))
            {
                OnFileRemoved(this, new ProjectFileRemovedEventArgs(evt.Project, rgpszMkDocuments[evt.ItemIndex]));
            }
            return VSConstants.S_OK;
        }

        public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
        {
            return VSConstants.S_OK;
        }

        public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
        {
            foreach (var evt in GetEvents(cProjects, rgpProjects, rgFirstIndices, rgszMkOldNames.Length))
            {
                OnFileRenamed(this, new ProjectFileRenamedEventArgs(evt.Project, rgszMkOldNames[evt.ItemIndex], rgszMkNewNames[evt.ItemIndex]));
            }
            return VSConstants.S_OK;
        }

        public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
        {
            return VSConstants.S_OK;
        }

        public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
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
