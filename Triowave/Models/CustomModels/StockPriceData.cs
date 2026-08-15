using System.Text.Json.Serialization;

namespace Triowave.Models.CustomModels
{
    public class StockPriceData
    {
        [JsonPropertyName("Meta Data")]
        public StockPriceMetaData MetaData { get; set; }

        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<DateTime, DailyStockPrice> TimeSeries { get; set; }

    }

    public class StockPriceMetaData
    {
        [JsonPropertyName("1. Information")]
        public string Information { get; set; }

        [JsonPropertyName("2. Symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("3. Last Refreshed")]
        public DateTime LastRefreshed { get; set; }

        [JsonPropertyName("4. Output Size")]
        public string OutputSize { get; set; }

        [JsonPropertyName("5. Time Zone")]
        public string TimeZone { get; set; }
    }

    public class DailyStockPrice
    {
        [JsonPropertyName("1. open")]
        public decimal Open { get; set; }

        [JsonPropertyName("2. high")]
        public decimal High { get; set; }

        [JsonPropertyName("3. low")]
        public decimal Low { get; set; }

        [JsonPropertyName("4. close")]
        public decimal Close { get; set; }

        [JsonPropertyName("5. volume")]
        public int Volume { get; set; }
    }
}
