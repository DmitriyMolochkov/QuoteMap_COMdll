using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace QuoteMap.TradingViewConnection.Messages
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TradingViewMsgType
    {
        [EnumMember(Value = "set_data_quality")]
        SetDataQuality,
        
        [EnumMember(Value = "quote_create_session")]
        QuoteCreateSession,
        
        [EnumMember(Value = "quote_set_fields")]
        QuoteSetFields,
        
        [EnumMember(Value = "quote_add_symbols")]
        QuoteAddSymbols,
        
        [EnumMember(Value = "quote_remove_symbols")]
        QuoteRemoveSymbols,
        
        [EnumMember(Value = "qsd")]
        QuoteUpdated,
        
        [EnumMember(Value = "quote_completed")]
        QuoteCompleted,
    }
}