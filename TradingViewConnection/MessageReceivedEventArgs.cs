using System;
using QuoteMap.TradingViewConnection.Messages;

namespace QuoteMap.TradingViewConnection
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public TradingViewMessage Message { get; }

        public MessageReceivedEventArgs(TradingViewMessage message)
        {
            Message = message;
        }
    }
}