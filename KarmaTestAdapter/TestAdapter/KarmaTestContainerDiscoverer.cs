using JsTestAdapter.Helpers;
using JsTestAdapter.Logging;
using JsTestAdapter.TestAdapter;
using KarmaTestAdapter.Helpers;
using KarmaTestAdapter.Logging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TPL = System.Threading.Tasks;

namespace KarmaTestAdapter.TestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class KarmaTestContainerDiscoverer : TestContainerDiscoverer
    {
        [ImportingConstructor]
        public KarmaTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            ITestsService testsService,
            KarmaTestSettingsProvider testSettingsService,
            ILogger logger
            )
            : base(serviceProvider, testsService, testSettingsService, new KarmaLogger(logger, true))
        {
        }

        public override TestAdapterInfo CreateTestAdapterInfo()
        {
            return new KarmaTestAdapterInfo();
        }

        public override TestContainer CreateTestContainer(TestContainerSource source)
        {
            return new KarmaTestContainer(Containers, source.Project, source.Source);
        }

        public override TestContainerSource CreateTestContainerSource(IVsProject project, string source)
        {
            return new KarmaTestContainerSource(project, source);
        }
    }
}