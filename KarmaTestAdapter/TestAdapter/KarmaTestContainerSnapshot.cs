using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KarmaTestAdapter.TestAdapter
{
    public class KarmaTestContainerSnapshot<TDiscoverer> : KarmaTestContainerBase<TDiscoverer>
        where TDiscoverer : ITestContainerDiscoverer
    {
        public KarmaTestContainerSnapshot(KarmaTestContainerBase<TDiscoverer> container)
            : base(container.Discoverer, container.Source, container.TimeStamp)
        {
        }
    }
}