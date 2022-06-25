using System.Runtime.InteropServices;

namespace QuoteMap
{
    [ComVisible(true)]
    [Guid("7e2fbb72-f4eb-48e6-9ff5-2a385c4fd474")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface IQuoteMapEvents
    {
        void QuoteUpdated(QuoteModel quote);
    }
}