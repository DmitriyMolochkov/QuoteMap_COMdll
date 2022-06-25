using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuoteMap.WebSocketClient
{
    public class WsClient : IDisposable
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private readonly ClientWebSocket _socket = new ClientWebSocket();
        private readonly Uri _uri;
        private bool _disposed;
        private static readonly bool IsHack = Hack(); // it's a crutch, don't touch it
        
        public WsClient(Uri uri)
        {
            _uri = uri;
        }
        
        public ClientWebSocketOptions Options => _socket.Options;

        public Task ConnectAsync() => _socket.ConnectAsync(_uri, CancellationToken.None);
        public Task SendAsync(string data) =>
            _socket.SendAsync(
                new ArraySegment<byte>(Encoding.UTF8.GetBytes(data)),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

        public void StartReceive()
        {
            Action receiveMessage = () =>
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(ReceiveAsync().GetAwaiter().GetResult()));
            
            IAsyncResult asyncResult = null;

            void Callback(IAsyncResult ar)
            {
                if (ar != null) receiveMessage.EndInvoke(ar);
                asyncResult = receiveMessage.BeginInvoke(Callback, asyncResult);
            }

            Callback(asyncResult);
        }

        private async Task<String> ReceiveAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);

            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    await ConnectAsync();

                ms.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                    return await reader.ReadToEndAsync();
            }
        }

        public async Task CloseAsync()
        {
            if (_socket.State == WebSocketState.Open)
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }
        
        private static bool Hack() //fix SetRequestHeader method of ClientWebSocketOptions https://github.com/dotnet/runtime/issues/24822
        {
            // use reflection to remove IsRequestRestricted from headerInfo hash table
            Assembly a = typeof(HttpWebRequest).Assembly;
            foreach (FieldInfo f in a.GetType("System.Net.HeaderInfoTable").GetFields(BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (f.Name == "HeaderHashTable")
                {
                    Hashtable hashTable = f.GetValue(null) as Hashtable;
                    foreach (string sKey in hashTable.Keys)
                    {
                        object headerInfo = hashTable[sKey];
                        //Console.WriteLine(String.Format("{0}: {1}", sKey, hashTable[sKey]));
                        foreach (FieldInfo g in a.GetType("System.Net.HeaderInfo").GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            if (g.Name == "IsRequestRestricted")
                            {
                                bool b = (bool)g.GetValue(headerInfo);
                                if (b)
                                {
                                    g.SetValue(headerInfo, false);
                                    //Console.WriteLine(sKey + "." + g.Name + " changed to false");
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        ~WsClient() => Dispose(false);
        
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _socket.Dispose();
            _disposed = true;
        }
    }
}