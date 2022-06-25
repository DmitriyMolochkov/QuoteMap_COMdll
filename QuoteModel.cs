using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace QuoteMap
{
    [ComVisible(true)]
    [Guid("39bc3ae9-f378-446d-8df4-83485a26f627")]
    [ClassInterface(ClassInterfaceType.None)]
    public class QuoteModel : IQuoteModel
    {
        public string Symbol { get; }
        public string Ticker => Regex.Split(Symbol, ":")[1];
        public double Price { get; }
        
        public QuoteModel(string symbol, double price)
        {
            Symbol = symbol;
            Price = price;
        }
        
        public QuoteModel(QuoteUpdateMessage message)
        {
            Symbol = message.Symbol;
            Price = message.Values.LastPrice;
        }
    }
}