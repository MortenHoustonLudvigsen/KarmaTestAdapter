using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarmaTestAdapter.Karma
{
    public class KarmaClient
    {
        public KarmaClient(int port, string host = null)
        {
            Port = port;
            Host = host;
            EventsCommand = new Lazy<KarmaEventCommand>(() => new KarmaEventCommand(port, host));
            StopCommand = new Lazy<KarmaStopCommand>(() => new KarmaStopCommand(port, host));
            DiscoverCommand = new Lazy<KarmaDiscoverCommand>(() => new KarmaDiscoverCommand(port, host));
        }

        public int Port { get; private set; }
        public string Host { get; private set; }

        public Lazy<KarmaEventCommand> EventsCommand { get; private set; }
        public Lazy<KarmaStopCommand> StopCommand { get; private set; }
        public Lazy<KarmaDiscoverCommand> DiscoverCommand { get; private set; }

        public async Task Stop()
        {
            await StopCommand.Value.Run();
        }
    }
}
