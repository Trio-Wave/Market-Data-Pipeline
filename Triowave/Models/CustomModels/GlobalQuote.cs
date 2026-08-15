using System.Text.Json.Serialization;

namespace Triowave.Models.CustomModels
{
    public class GlobalQuoteData
    {
        [JsonPropertyName("Global Quote")]
        public GlobalQuote GlobalQuote { get; set; }
    }

    public class GlobalQuote
    {
        [JsonPropertyName("01. symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("02. open")]
        public decimal? Open { get; set; }

        [JsonPropertyName("03. high")]
        public decimal? High { get; set; }

        [JsonPropertyName("04. low")]
        public decimal? Low { get; set; }

        [JsonPropertyName("05. price")]
        public decimal? Price { get; set; }

        [JsonPropertyName("06. volume")]
        public int? Volume { get; set; }

        [JsonPropertyName("07. latest trading day")]
        public DateTime Date { get; set; }
    }
}
