using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QuoteMap.TradingViewConnection.Messages
{
    public class SessionMessage
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
        
        [JsonProperty("timestamp")]
        public int Timestamp { get; set; }
        
        [JsonProperty("timestampMs")]
        public long TimestampMs { get; set; }
        
        [JsonProperty("release")]
        public string Release { get; set; }
        
        [JsonProperty("studies_metadata_hash")]
        public string StudiesMetadataHash { get; set; }
        
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        
        [JsonProperty("javastudies")]
        public string Javastudies { get; set; }
        
        [JsonProperty("auth_scheme_vsn")]
        public int AuthSchemeVsn { get; set; }
        
        [JsonProperty("via")]
        public string Via { get; set; }

        public static bool IsInstance(string json) => JObject.Parse(json).ContainsKey("session_id");
    }
}