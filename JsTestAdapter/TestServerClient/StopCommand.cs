using JsTestAdapter.JsonServerClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JsTestAdapter.TestServerClient
{
    public class StopCommand : JsonServerCommand
    {
        public StopCommand(int port, string host = null)
            : base("stop", port, host)
        {
        }

        public async Task Run()
        {
            await RunInternal();
        }
    }
}
