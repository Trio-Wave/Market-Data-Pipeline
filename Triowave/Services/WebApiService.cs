using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Triowave.Configuration;
using Triowave.Models.CustomModels;

namespace Triowave.Services
{
    public class WebApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettingsService _appSettings;
        private readonly ILogger<WebApiService> _logger;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public WebApiService(
            HttpClient httpClient,
            AppSettingsService appSettings,
            ILogger<WebApiService> logger)
        {
            _httpClient = httpClient;
            _appSettings = appSettings;
            _logger = logger;
        }

        public async Task<StockPriceData?> GetDailyStockTimeSeries(string symbol, string outputSize = "compact")
        {
            var alphaVantage = _appSettings.GetSection<AlphaVantageOptions>("AlphaVantage");

            var query = new Dictionary<string, string?>
            {
                ["function"] = "TIME_SERIES_DAILY",
                ["symbol"] = symbol,
                ["outputsize"] = outputSize,
                ["apikey"] = alphaVantage.ApiKey
            };

            var requestUri = QueryHelpers.AddQueryString(alphaVantage.BaseUrl, query);

            _logger.LogInformation("Requesting daily time series for {Symbol} with output size {OutputSize}.", symbol, outputSize);

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<StockPriceData>(json, JsonOptions);

                if (data?.TimeSeries is null)
                {
                    _logger.LogWarning("Daily time series response for {Symbol} did not contain time series data.", symbol);
                }

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve daily time series for {Symbol}.", symbol);
                throw;
            }
        }
    }
}
