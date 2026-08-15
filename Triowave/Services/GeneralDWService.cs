using Microsoft.EntityFrameworkCore;
using Triowave.Interfaces;
using Triowave.Models;
using Triowave.Models.CustomModels;

namespace Triowave.Services
{
    public class GeneralDWService : IGeneralDWService
    {

        private readonly GeneralDWContext _context;
        private readonly ILogger<GeneralDWService> _logger;

        public GeneralDWService(GeneralDWContext context, ILogger<GeneralDWService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get enabled symbols that have not been back-filled yet
        /// </summary>
        /// <param name="numSymbols"></param>
        /// <returns></returns>
        public async Task<List<string>> GetUnfilledSymbols(int numSymbols)
        {
            var filledSymbols = _context.StockPrices
                .Select(s => s.Symbol)
                .Distinct()
                .ToList();

            return await _context.Symbols
                .Where(s => s.Enabled == true && !filledSymbols.Contains(s.Symbol1))
                .OrderBy(s => s.Id)
                .Take(numSymbols)
                .Select(s => s.Symbol1)
                .ToListAsync();
        }

        public async Task<List<string>> GetEnabledSymbols()
        {
            return await _context.Symbols
                .Where(s => s.Enabled == true)
                .OrderBy(s => s.Id)
                .Select(s => s.Symbol1)
                .ToListAsync();
        }

        /// <summary>
        /// Checks which symbols have been updated with recent data 
        /// </summary>
        /// <returns>list of enabled symbols that have not been filled yet</returns>
        public async Task<List<string>> GetDailySyncSymbols()
        {
            var enabledSymbols = await GetEnabledSymbols();

            var checkDate = DateTime.Now.Hour > 17 ? DateTime.Now.Date : DateTime.Now.AddDays(-1).Date;

            var filledSymbols = await _context.StockPrices
                .Where(s => s.Date.Date == checkDate)
                .Select(s => s.Symbol)
                .Distinct()
                .ToListAsync();

            return enabledSymbols
                .Where(symbol => !filledSymbols.Contains(symbol))
                .ToList();
        }

        public IQueryable<GlobalQuote> GetGlobalQuotes(string symbol)
        {
            return _context.StockPrices
                .Where(s => s.Symbol == symbol)
                .Select(s => new GlobalQuote
                {
                    Date = s.Date,
                    Open = s.Open,
                    High = s.High,
                    Low = s.Low,
                    Price = s.Close,
                    Volume = s.Volume
                }).AsQueryable().OrderByDescending(x => x.Date);
        }

        public async Task StoreGlobalQuote(string symbol, GlobalQuote globalQuote)
        {

            try
            {
                var existingStockPrice = await _context.StockPrices
                    .Where(x => x.Symbol == symbol && x.Date == globalQuote.Date)
                    .FirstOrDefaultAsync();

                var dbGlobalQuote = existingStockPrice ?? new StockPrice { Symbol = symbol };

                dbGlobalQuote.Date = globalQuote.Date;
                dbGlobalQuote.Open = globalQuote.Open;
                dbGlobalQuote.High = globalQuote.High;
                dbGlobalQuote.Low = globalQuote.Low;
                dbGlobalQuote.Close = globalQuote.Price;
                dbGlobalQuote.Volume = globalQuote.Volume;

                if (existingStockPrice is null) _context.Add(dbGlobalQuote);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing stock prices for {Symbol}.", symbol);
                throw;
            }
        }

        public async Task StoreStockPrices(string symbol, StockPriceData stockPriceData)
        {
            if (stockPriceData.TimeSeries is null || stockPriceData.TimeSeries.Count == 0)
            {
                _logger.LogWarning("No time series values to store for {Symbol}.", symbol);
                return;
            }

            try
            {
                var existingPrices = await _context.StockPrices
                    .Where(price => price.Symbol == symbol)
                    .ToListAsync();

                var existingByDate = existingPrices.ToDictionary(price => price.Date.Date);

                foreach (var (date, dailyPrice) in stockPriceData.TimeSeries)
                {
                    var tradeDate = date.Date;

                    if (existingByDate.TryGetValue(tradeDate, out var existingPrice))
                    {
                        existingPrice.Open = dailyPrice.Open;
                        existingPrice.High = dailyPrice.High;
                        existingPrice.Low = dailyPrice.Low;
                        existingPrice.Close = dailyPrice.Close;
                        existingPrice.Volume = dailyPrice.Volume;
                    }
                    else
                    {
                        _context.StockPrices.Add(new StockPrice
                        {
                            Symbol = symbol,
                            Date = tradeDate,
                            Open = dailyPrice.Open,
                            High = dailyPrice.High,
                            Low = dailyPrice.Low,
                            Close = dailyPrice.Close,
                            Volume = dailyPrice.Volume
                        });
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Stored {Count} daily prices for {Symbol}.", stockPriceData.TimeSeries.Count, symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing stock prices for {Symbol}.", symbol);
                throw;
            }
        }

        public async Task StoreSymbols(SymbolData symbol)
        {
            try
            {
                var existingSymbol = await _context.Symbols.Where(x => x.Symbol1 == symbol.Symbol).FirstOrDefaultAsync();

                var newSymbol = existingSymbol is null ? new Symbol() { Symbol1 = symbol.Symbol } : existingSymbol;

                newSymbol.Name = symbol.Name;
                newSymbol.Exchange = symbol.Exchange;
                newSymbol.AssetType = symbol.AssetType;
                newSymbol.IpoDate = symbol.IPODate;
                newSymbol.Status = symbol.Status == "Active";
                newSymbol.Enabled = false;

                if (existingSymbol is null) _context.Symbols.Add(newSymbol);
                else _context.Symbols.Update(newSymbol);

                // Add new symbols
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing symbol {Symbol}.", symbol.Symbol);
            }
        }
    }
}
