using JsTestAdapter.JsonServerClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Karma
{
    public class KarmaDiscoverCommand : JsonServerCommand
    {
        public KarmaDiscoverCommand(int port, string host = null)
            : base("discover", port, host)
        {
        }

        public async Task Run(Action<KarmaSpec> onSpec)
        {
            await RunInternal(message => onSpec(message.ToObject<KarmaSpec>()));
        }
    }
}
