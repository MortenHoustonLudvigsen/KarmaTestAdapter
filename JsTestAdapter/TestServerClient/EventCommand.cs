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
    public class EventCommand : JsonServerCommand
    {
        public EventCommand(int port, string host = null)
            : base("event", port, host)
        {
        }

        public async Task Run(Action<ServerEvent> onEvent)
        {
            await RunInternal(message => onEvent(message.ToObject<ServerEvent>()));
        }
    }
}
