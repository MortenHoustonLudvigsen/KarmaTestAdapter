using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsTestAdapter.TestServer
{
    public class ServerEvent
    {
        public string Event { get; set; }
        public IEnumerable<Guid> Tests { get; set; }

        public override string ToString()
        {
            return Event;
        }
    }
}