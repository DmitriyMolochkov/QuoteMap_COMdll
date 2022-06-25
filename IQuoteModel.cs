using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace QuoteMap
{
    [ComVisible(true)]
    [Guid("dfb77a2f-0ba8-460a-8d78-60a12d5b9422")]
    public interface IQuoteModel
    {
        string Symbol { get; }
        string Ticker { get; }
        double Price { get; }
    }
}