using JsTestAdapter.Helpers;
using JsTestAdapter.TestAdapter;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestContainerSource : TestContainerSource
    {
        public KarmaTestContainerSource(IVsProject project, string source)
            : base(project, GetSource(project, source))
        {
        }

        private static string GetSource(IVsProject project, string source)
        {
            var directory = Directory.Exists(source) ? source : Path.GetDirectoryName(source);
            var candidates = new[]{
                Path.Combine(directory, Globals.SettingsFilename),
                Path.Combine(directory, Globals.KarmaConfigFilename)
            };
            return candidates.FirstOrDefault(f => project.HasFile(f));
        }
    }
}