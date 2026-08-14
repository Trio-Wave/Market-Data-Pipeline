using Triowave.Models.CustomModels;

namespace Triowave.Interfaces
{
    public interface IWebApiService
    {
        public Task<StockPriceData?> GetDailyStockTimeSeries(string symbol, int apiKeyNumber);

        public Task<GlobalQuote?> GetDailyStockQuote(string symbol, int apiKeyNumber);
    }
}
