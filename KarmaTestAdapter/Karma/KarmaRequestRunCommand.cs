using JsonServerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KarmaTestAdapter.Karma
{
    public class KarmaRequestRunCommand : JsonServerCommand
    {
        public KarmaRequestRunCommand(int port, string host = null)
            : base("requestRun", port, host)
        {
        }

        public async Task Run(IEnumerable<Guid> tests)
        {
            await RunInternal(new { command = CommandName, tests = tests.ToList() });
        }
    }
}