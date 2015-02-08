using KarmaTestAdapter.TestResults;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public class KarmaTestContainerSnapshot : KarmaTestContainerBase
    {
        public KarmaTestContainerSnapshot(IKarmaTestContainer container)
            : base(container.KarmaTestContainerDiscoverer, container.Source, container.TimeStamp)
        {
        }
    }
}
