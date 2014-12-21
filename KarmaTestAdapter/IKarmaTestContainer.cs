using KarmaTestAdapter.KarmaTestResults;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter
{
    public interface IKarmaTestContainer : ITestContainer
    {
        DateTime TimeStamp { get; }
        KarmaTestContainerDiscoverer KarmaTestContainerDiscoverer { get; }
    }
}
