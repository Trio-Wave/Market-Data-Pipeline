using Microsoft.AspNetCore.Mvc;
using Triowave.Configuration;
using Triowave.Interfaces;
using Triowave.Models.CustomModels;

namespace Triowave.Services
{
    public class MarketDataService : IMarketDataService
    {
        private readonly IGeneralDWService _generalDwService;
        private readonly IWebApiService _webApiService;
        private readonly ILogger<MarketDataService> _logger;
        private readonly AppSettingsService _appSettings;

        public MarketDataService(
            IGeneralDWService generalDwService,
            IWebApiService webApiService,
            AppSettingsService appSettings,
            ILogger<MarketDataService> logger)
        {
            _generalDwService = generalDwService;
            _webApiService = webApiService;
            _appSettings = appSettings;
            _logger = logger;
        }

        #region Helper Methods

        private async Task<DataSyncResult> StoreRequestedData<T>(
    List<string> symbols,
    Func<string, int, Task<T>> getStockFromApi,
    Func<string, T, Task> storeStockData)
        {
            var alphaVantage = _appSettings.GetSection<AlphaVantageOptions>("AlphaVantage");
            var totalKeys = alphaVantage.NumberOfApiKeys;

            var currentKey = 1;
            var storedCount = 0;
            var failedCount = 0;

            for (var i = 0; i < symbols.Count; i++)
            {
                var symbol = symbols[i];

                if (currentKey > totalKeys)
                {
                    _logger.LogError(
                        "All {TotalKeys} API keys exhausted; stopping sync with {Remaining} symbols unprocessed starting at {Symbol}.",
                        totalKeys, symbols.Count - i, symbol);

                    failedCount += symbols.Count - i;
                    break;
                }

                var (stockData, nextKey) = await FetchWithKeyRotation(symbol, getStockFromApi, currentKey, totalKeys);
                currentKey = nextKey;

                if (stockData is null)
                {
                    failedCount++;
                }
                else
                {
                    try
                    {
                        await storeStockData(symbol, stockData);
                        storedCount++;
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        _logger.LogError(ex, "Failed to store stock data for {Symbol}.", symbol);
                    }
                }

                await Task.Delay(1000); // Don't overload api
            }

            return new DataSyncResult
            {
                TotalCount = symbols.Count,
                StoredCount = storedCount,
                FailedCount = failedCount
            };
        }

        private async Task<(T? Data, int NextKey)> FetchWithKeyRotation<T>(
            string symbol,
            Func<string, int, Task<T>> getStockFromApi,
            int currentKey,
            int totalKeys)
        {
            while (currentKey <= totalKeys)
            {
                try
                {
                    var data = await getStockFromApi(symbol, currentKey);
                    if (data is not null) return (data, currentKey);

                    _logger.LogWarning("No data returned for {Symbol} using key {Key}.", symbol, currentKey);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "API call failed for {Symbol} using key {Key}.", symbol, currentKey);
                }

                currentKey++;
            }

            return (default, currentKey);
        }

        #endregion

        #region AlphaVantage API Calls
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<DataSyncResult> BackfillStockData()
        {
            var symbols = await _generalDwService.GetUnfilledSymbols(25);

            return await StoreRequestedData(
                symbols,
                _webApiService.GetDailyStockTimeSeries,
                _generalDwService.StoreStockPrices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<DataSyncResult> PullDailyStockQuote()
        {
            var symbols = await _generalDwService.GetDailySyncSymbols();

            return await StoreRequestedData(
                symbols,
                _webApiService.GetDailyStockQuote,
                _generalDwService.StoreGlobalQuote);
        }

        #endregion
    }
}
