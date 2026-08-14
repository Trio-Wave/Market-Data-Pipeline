using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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

            foreach (var symbol in symbols)
            {
                try
                {
                    var stockPriceData = await getStockFromApi(symbol, currentKey);

                    if (stockPriceData is null)
                    {
                        failedCount++;
                        _logger.LogWarning("No data returned for {Symbol}.", symbol);

                        currentKey++;
                        if (currentKey > totalKeys) break;
                        continue;
                    }

                    await storeStockData(symbol, stockPriceData);
                    storedCount++;

                    await Task.Delay(1000); // Don't overload api
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _logger.LogError(ex, "Failed to sync stock data for {Symbol}.", symbol);

                    currentKey++;
                    if (currentKey > totalKeys) break;
                }

            }

            return new DataSyncResult
            {
                TotalCount = symbols.Count,
                StoredCount = storedCount,
                FailedCount = failedCount
            };
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
            var symbols = await _generalDwService.GetEnabledSymbols();

            return await StoreRequestedData(
                symbols,
                _webApiService.GetDailyStockQuote,
                _generalDwService.StoreGlobalQuote);
        }

        #endregion
    }
}
