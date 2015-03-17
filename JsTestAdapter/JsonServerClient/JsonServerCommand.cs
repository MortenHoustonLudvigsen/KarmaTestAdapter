using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JsTestAdapter.JsonServerClient
{
    public class JsonServerCommand
    {
        public JsonServerCommand(string commandName, int port, string host)
        {
            CommandName = commandName;
            Port = port;
            Host = host ?? IPAddress.Loopback.ToString();
        }

        public string CommandName { get; private set; }
        public string Host { get; private set; }
        public int Port { get; private set; }

        public event Action Connected;
        private void onConnected()
        {
            if (Connected != null)
            {
                Connected();
            }
        }

        public event Action Disconnected;
        private void onDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected();
            }
        }

        public event Action<JObject> MessageReceived;
        private void onMessageReceived(JObject message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(message);
            }
        }

        protected async Task RunInternal(Action<JObject> onMessage = null)
        {
            await RunInternal(new { command = CommandName }, onMessage);
        }

        protected async Task RunInternal(object request, Action<JObject> onMessage = null)
        {
            using (var client = new JsonServerClient(Host, Port))
            {
                var tcs = new TaskCompletionSource<bool>();
                try
                {
                    client.Connected += onConnected;
                    client.Disconnected += onDisconnected;
                    client.Disconnected += () =>
                    {
                        tcs.TrySetResult(true);
                    };
                    client.MessageRecieved += message =>
                    {
                        try
                        {
                            onMessageReceived(message);
                            if (message.GetValue<bool>("FINISHED"))
                            {
                                client.Disconnect();
                                tcs.TrySetResult(true);
                            }
                            else if (onMessage != null)
                            {
                                onMessage(message);
                            }
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                            client.Disconnect();
                        }
                    };
                    client.Connect();
                    client.Write(request);
                }
                catch (Exception ex)
                {
                    tcs.TrySetResult(true);
                    throw ex;
                }

                await tcs.Task;
            }
        }
    }
}
