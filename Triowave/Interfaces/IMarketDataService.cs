using Triowave.Models.CustomModels;

namespace Triowave.Interfaces
{
    public interface IMarketDataService
    {
        Task<DataSyncResult> PullDailyStockQuote();

        Task<DataSyncResult> BackfillStockData();
    }
}
