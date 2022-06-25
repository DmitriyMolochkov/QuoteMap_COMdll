using Newtonsoft.Json;

namespace QuoteMap.TradingViewConnection.Messages
{
    public class TradingViewMessage
    {
        [JsonProperty("m")]
        public TradingViewMsgType Type { get; }
        
        [JsonProperty("p")]
        public object[] Parameters { get; }
        
        public TradingViewMessage(TradingViewMsgType type, object[] parameters)
        {
            Type = type;
            Parameters = parameters;
        }
    }
}