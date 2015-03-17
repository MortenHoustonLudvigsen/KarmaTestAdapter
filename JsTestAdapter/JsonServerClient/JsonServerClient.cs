using ReactiveSockets;
using System.Reactive.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace JsTestAdapter.JsonServerClient
{
    public class JsonServerClient : IDisposable
    {
        private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None
        };

        private static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSettings);
        }

        private static JObject Deserialize(string json)
        {
            using(var textReader = new StringReader(json))
            using(var jsonReader = new JsonTextReader(textReader))
            {
                jsonReader.DateParseHandling = DateParseHandling.DateTime;
                return JObject.Load(jsonReader);
            }
        }

        public JsonServerClient(string host, int port)
        {
            Host = host;
            Port = port;
            Client = new ReactiveClient(Host, Port);

            Client.Connected += OnClientConnected;
            Client.Disconnected += OnClientDisconnected;

            Client.Receiver.Buffer(1)
                .Select(b => b.First())
                .SubscribeOn(TaskPoolScheduler.Default)
                .Subscribe(OnByteRecieved);
        }

        public string Host { get; private set; }
        public int Port { get; private set; }
        public ReactiveClient Client { get; private set; }
        public bool IsConnected { get { return Client.IsConnected; } }

        public event Action<JObject> MessageRecieved;
        private void OnMessageRecieved(string message)
        {
            var parsedMessage = Deserialize(message);
            if (MessageRecieved != null)
            {
                MessageRecieved(parsedMessage);
            }
        }

        private Buffer _buffer = new Buffer();
        private void OnByteRecieved(byte b)
        {
            if (b == 0)
            {
                if (_buffer.Count > 0)
                {
                    OnMessageRecieved(_buffer.ToString());
                }
                _buffer.Clear();
            }
            else
            {
                _buffer.Add(b);
            }
        }

        public event Action Connected;
        private void OnClientConnected(object sender, EventArgs e)
        {
            if (Connected != null)
            {
                Connected();
            }
        }

        public event Action Disconnected;
        private void OnClientDisconnected(object sender, EventArgs e)
        {
            if (Disconnected != null)
            {
                Disconnected();
            }
        }

        public void Connect()
        {
            Client.ConnectAsync().Wait();
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                Client.Disconnect();
            }
        }

        public void Write(object message)
        {
            Client.SendAsync(Encoding.UTF8.GetBytes(Serialize(message)).Concat(new byte[] { 0 }).ToArray()).Wait();
        }

        #region IDisposable

        // Flag: Has Dispose already been called? 
        bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern. 
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Client.Dispose();
            }

            _disposed = true;
        }

        ~JsonServerClient()
        {
            Dispose(false);
        }

        #endregion
    }
}
