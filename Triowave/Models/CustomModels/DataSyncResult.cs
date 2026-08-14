namespace Triowave.Models.CustomModels
{
    public class DataSyncResult
    {
        public int TotalCount { get; init; }
        public int StoredCount { get; init; }
        public int FailedCount { get; init; }

        public bool IsSuccess =>
            FailedCount == 0 && StoredCount > 0;
    }
}
