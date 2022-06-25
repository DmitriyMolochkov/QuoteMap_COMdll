using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuoteMap.WebSocketClient;
using QuoteMap.TradingViewConnection.Messages;

namespace QuoteMap.TradingViewConnection
{
    public class Connection : IDisposable
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        private readonly WsClient _wsClient;
        private SessionMessage _sessionMessage;
        private bool _isOpen;
        private bool _disposed;
        
        public Connection() 
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _wsClient = new WsClient(new Uri("wss://prodata.tradingview.com/socket.io/websocket"));
            _wsClient.Options.SetRequestHeader("origin", "https://prodata.tradingview.com");
            _wsClient.MessageReceived += (_, eventArgs) => HandleMessages(eventArgs.Message);
        }

        public async Task ConnectionAsync()
        {
            await _wsClient.ConnectAsync();
            _wsClient.StartReceive();
            await Task.Run(async () =>
                {
                    while (!_isOpen) 
                        await Task.Delay(100);
                }
            );
            await SendAsync(new TradingViewMessage(TradingViewMsgType.SetDataQuality, new object[] { "low" }));
        }

        public async Task SendAsync(TradingViewMessage message)
        {
            string json = JsonConvert.SerializeObject(message);
            await _wsClient.SendAsync($"~m~{json.Length}~m~{json}");
        }

        private async Task HandleMessage(string json)
        {
            if (PingMessage.IsInstance(json))
            {
                await _wsClient.SendAsync(new PingMessage(json).Data);
            }
            else if (SessionMessage.IsInstance(json))
            {
                _sessionMessage = JsonConvert.DeserializeObject<SessionMessage>(json);
                _isOpen = true;
            }
            else
            {
                var message = JsonConvert.DeserializeObject<TradingViewMessage>(json);
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
            }
        }

        private Task HandleMessages(string json) =>
            Task.WhenAll(
                Regex
                    .Split(json, "~m~\\d+~m~")
                    .ToList()
                    .Select(HandleMessage)
                    .ToArray()
            );

        public async Task CloseAsync() => await _wsClient.CloseAsync();

        ~Connection() => Dispose(false);
        
        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _wsClient.Dispose();
            _disposed = true;
        }
    }
}