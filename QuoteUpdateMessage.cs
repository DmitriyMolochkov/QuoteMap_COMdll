using Newtonsoft.Json;

namespace QuoteMap
{
    public class QuoteUpdateMessage
    {
        [JsonProperty("n")]
        public string Symbol { get; set; }
        
        [JsonProperty("s")]
        public string Status { get; set; }
        
        [JsonProperty("v")]
        public QuoteValues Values { get; set; }
    }

    public class QuoteValues
    {
        [JsonProperty("lp")]
        public double LastPrice { get; set; }
    }
}