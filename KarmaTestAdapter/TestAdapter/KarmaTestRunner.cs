using JsTestAdapter.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.TestAdapter
{
    [FileExtension(".js")]
    [FileExtension(".json")]
    [DefaultExecutorUri(Globals.ExecutorUriString)]
    [ExtensionUri(Globals.ExecutorUriString)]
    public class KarmaTestRunner : TestRunner
    {
        public override TestAdapterInfo CreateTestAdapterInfo()
        {
            return new KarmaTestAdapterInfo();
        }
    }
}
