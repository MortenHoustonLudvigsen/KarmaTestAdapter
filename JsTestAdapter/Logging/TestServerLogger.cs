using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsTestAdapter.Logging
{
    public abstract class TestServerLogger : TestLogger
    {
        public TestServerLogger(ITestLogger logger, params string[] context)
            : base(logger, context)
        {
        }

        public abstract void Log(string message);
    }
}