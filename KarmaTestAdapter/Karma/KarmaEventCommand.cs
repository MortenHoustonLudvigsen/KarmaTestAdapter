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
    public class KarmaEventCommand : JsonServerCommand
    {
        public KarmaEventCommand(int port, string host = null)
            : base("event", port, host)
        {
        }

        public async Task Run(Action<KarmaEvent> onEvent)
        {
            await RunInternal(message => onEvent(message.ToObject<KarmaEvent>()));
        }
    }
}
