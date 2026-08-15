namespace Triowave.Configuration
{
    public class AlphaVantageOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string ApiKey1 { get; set; } = string.Empty;

        public string ApiKey2 { get; set; } = string.Empty;

        public string ApiKey3 { get; set; } = string.Empty;

        public string ApiKey4 { get; set; } = string.Empty;

        public int NumberOfApiKeys { get; set; } = 1;
    }
}
