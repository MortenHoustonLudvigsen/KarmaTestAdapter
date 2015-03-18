using JsTestAdapter.JsonServerClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.TestServerClient
{
    public class DiscoverCommand : JsonServerCommand
    {
        public DiscoverCommand(int port, string host = null)
            : base("discover", port, host)
        {
        }

        public async Task Run(Action<Spec> onSpec)
        {
            await RunInternal(message => onSpec(message.ToObject<Spec>()));
        }
    }
}
