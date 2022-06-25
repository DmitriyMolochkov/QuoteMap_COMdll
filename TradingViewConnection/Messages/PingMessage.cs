namespace QuoteMap.TradingViewConnection.Messages
{
    public class PingMessage
    {
        public string Data { get; }
        
        public PingMessage(string message)
        {
            Data = $"~m~{message.Length}~m~{message}";
        }

        public static bool IsInstance(string message) => message.Substring(0, 3) == "~h~";
    }
}