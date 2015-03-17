using JsTestAdapter.Helpers;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace JsTestAdapter.TestAdapter
{
    public class TestContainerSource
    {
        public TestContainerSource(IVsProject project, string source)
        {
            Project = project;
            Source = PathUtils.GetPhysicalPath(source);
            SourceDirectory = Path.GetDirectoryName(Source);
        }

        public IVsProject Project { get; private set; }
        public string SourceDirectory { get; private set; }
        public string Source { get; private set; }

        public override string ToString()
        {
            return Source;
        }
    }
}