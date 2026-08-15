using Triowave.Models.CustomModels;

namespace Triowave.Interfaces
{
    public interface IGeneralDWService
    {
        public Task<List<string>> GetUnfilledSymbols(int numSymbols);
        public Task<List<string>> GetEnabledSymbols();
        public Task<List<string>> GetDailySyncSymbols();
        public IQueryable<GlobalQuote> GetGlobalQuotes(string symbol);
        public Task StoreGlobalQuote(string symbol, GlobalQuote globalQuote);
        public Task StoreStockPrices(string symbol, StockPriceData stockPriceData);
        public Task StoreSymbols(SymbolData symbol);
    }
}
