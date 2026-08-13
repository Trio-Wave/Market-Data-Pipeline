using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Triowave.Models.CustomModels;
using Triowave.Services;

namespace Triowave.Controllers
{
    public class MarketDataController : Controller
    {
        private readonly GeneralDWService _generalDwService;
        private readonly WebApiService _webApiService;
        private readonly ILogger<MarketDataController> _logger;

        public MarketDataController(
            GeneralDWService generalDwService,
            WebApiService webApiService,
            ILogger<MarketDataController> logger)
        {
            _generalDwService = generalDwService;
            _webApiService = webApiService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task StoreSymbols()
        {
            try
            {
                string filePath = Path.Combine("C:\\Users\\olivi\\gitHubProjects\\Triowave\\Triowave\\Content\\", "activeSymbols.json");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogError("JSON file not found at {FilePath}.", filePath);
                    return;
                }

                string jsonString = System.IO.File.ReadAllText(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var symbolList = JsonSerializer.Deserialize<List<SymbolData>>(jsonString, options);

                foreach (var symbol in symbolList)
                {
                    await _generalDwService.StoreSymbols(symbol);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while storing symbols.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while storing symbols.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SyncStockData()
        {
            var symbols = await _generalDwService.GetUnfilledSymbols(5);
            var storedCount = 0;
            var failedCount = 0;

            foreach (var symbol in symbols)
            {
                try
                {
                    var stockPriceData = await _webApiService.GetDailyStockTimeSeries(symbol.Symbol1);

                    if (stockPriceData?.TimeSeries is null)
                    {
                        failedCount++;
                        _logger.LogWarning("No time series data returned for {Symbol}.", symbol.Symbol1);
                        continue;
                    }

                    await _generalDwService.StoreStockPrices(symbol.Symbol1, stockPriceData);
                    storedCount++;
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _logger.LogError(ex, "Failed to sync stock data for {Symbol}.", symbol.Symbol1);
                }
            }

            TempData["Message"] = $"Synced {storedCount} of {symbols.Count} symbols. Failed: {failedCount}.";
            TempData["IsSuccess"] = failedCount == 0 && storedCount > 0;
            return RedirectToAction(nameof(Index));
        }
    }
}
