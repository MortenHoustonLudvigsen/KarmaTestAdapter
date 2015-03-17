using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsTestAdapter.TestAdapter
{
    public class TestContainerSnapshot : TestContainerBase
    {
        public TestContainerSnapshot(TestContainerBase container)
            : base(container.Discoverer, container.Source, container.TimeStamp)
        {
        }
    }
}