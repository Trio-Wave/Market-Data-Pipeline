using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Text.Json.Serialization;
using Triowave.Configuration;
using Triowave.Interfaces;
using Triowave.Models.CustomModels;

namespace Triowave.Services
{
    public class WebApiService : IWebApiService
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

        private async Task<T?> GetApiResponse<T>(string symbol, int apiKeyNumber, string function)
        {
            var requestUri = BuildUri(symbol, function, apiKeyNumber);

            _logger.LogInformation("Requesting daily time series for {Symbol}.", symbol);

            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<T>(json, JsonOptions);

                if (data is null)
                {
                    _logger.LogWarning("{function} response for {Symbol} did not contain data.", function, symbol);
                }

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve {function} for {Symbol}.", function, symbol);
                throw;
            }
        }

        private string BuildUri(string symbol, string function, int apiKeyNumber)
        {
            var alphaVantage = _appSettings.GetSection<AlphaVantageOptions>("AlphaVantage");

            var query = new Dictionary<string, string?>
            {
                ["function"] = function,
                ["symbol"] = symbol,
                ["apikey"] = apiKeyNumber switch
                {
                    1 => alphaVantage.ApiKey1,
                    2 => alphaVantage.ApiKey2,
                    3 => alphaVantage.ApiKey3,
                    _ => alphaVantage.ApiKey1
                }
            };

            return QueryHelpers.AddQueryString(alphaVantage.BaseUrl, query);
        }

        public async Task<GlobalQuote?> GetDailyStockQuote(string symbol, int apiKeyNumber)
        {
            var globalQuoteData = await GetApiResponse<GlobalQuoteData>(symbol, apiKeyNumber, "GLOBAL_QUOTE");

            if (globalQuoteData?.GlobalQuote is null)
            {
                _logger.LogWarning("Global Quote response for {Symbol} did not contain data.", symbol);
            }

            return globalQuoteData?.GlobalQuote;

        }

        public async Task<StockPriceData?> GetDailyStockTimeSeries(string symbol, int apiKeyNumber)
        {
            return await GetApiResponse<StockPriceData>(symbol, apiKeyNumber, "TIME_SERIES_DAILY");
        }
    }
}
