using JsTestAdapter.JsonServerClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Karma
{
    public class KarmaStopCommand : JsonServerCommand
    {
        public KarmaStopCommand(int port, string host = null)
            : base("stop", port, host)
        {
        }

        public async Task Run()
        {
            await RunInternal();
        }
    }
}
