using JsTestAdapter.JsonServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace JsTestAdapter.TestServerClient
{
    public class RequestRunCommand : JsonServerCommand
    {
        public RequestRunCommand(int port, string host = null)
            : base("requestRun", port, host)
        {
        }

        public async Task Run(IEnumerable<Guid> tests)
        {
            await RunInternal(new { command = CommandName, tests = tests.ToList() });
        }
    }
}