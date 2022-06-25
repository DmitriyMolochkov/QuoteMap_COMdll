using System;

namespace QuoteMap.WebSocketClient
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }
        public MessageReceivedEventArgs(string message) => Message = message;
    }
}