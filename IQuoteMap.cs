using System.Runtime.InteropServices;

namespace QuoteMap
{
    [ComVisible(true)]
    [Guid("2f8ec22d-7864-43f1-a6fb-ae9fcf1de160")]
    public interface IQuoteMap
    {
        event QuoteEventHandler QuoteUpdated;
        
        void StartSessionSync();
        QuoteModel GetQuote(string symbol);
        bool AddQuoteSync(string symbol);
        bool DeleteQuoteSync(string symbol);
        void ClearQuotesSync();
    }
}