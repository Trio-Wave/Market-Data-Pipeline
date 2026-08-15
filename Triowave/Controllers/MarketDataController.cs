using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Triowave.Interfaces;
using Triowave.Models.CustomModels;
using Triowave.Models.ViewModels;

namespace Triowave.Controllers
{
    public class MarketDataController : Controller
    {
        private readonly IGeneralDWService _generalDwService;
        private readonly IMarketDataService _marketDataService;
        private readonly ILogger<MarketDataController> _logger;

        public MarketDataController(
            IGeneralDWService generalDwService,
            IMarketDataService marketDataService,
            ILogger<MarketDataController> logger)
        {
            _generalDwService = generalDwService;
            _marketDataService = marketDataService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? symbol)
        {
            var symbols = await _generalDwService.GetEnabledSymbols();

            var selectedSymbol = string.IsNullOrWhiteSpace(symbol)
                ? symbols.FirstOrDefault() ?? "AAPL"
                : symbol.ToUpper();

            var globalQuotes =
                _generalDwService.GetGlobalQuotes(selectedSymbol);

            var model = new MarketDataViewModel
            {
                GlobalQuotes = globalQuotes,
                Symbols = symbols,
                SelectedSymbol = selectedSymbol
            };

            return View(model);
        }

        #region CSV Handling

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

        #endregion

        #region AlphaVantage API Sync
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BackfillStockData()
        {
            var result = await _marketDataService.BackfillStockData();

            TempData["Message"] = $"Synced {result.StoredCount} of {result.TotalCount} symbols. Failed: {result.FailedCount}.";
            TempData["IsSuccess"] = result.FailedCount == 0 && result.StoredCount > 0;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PullDailyStockQuote()
        {
            var result = await _marketDataService.PullDailyStockQuote();

            TempData["Message"] = $"Synced {result.StoredCount} of {result.TotalCount} symbols. Failed: {result.FailedCount}.";
            TempData["IsSuccess"] = result.FailedCount == 0 && result.StoredCount > 0;
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}