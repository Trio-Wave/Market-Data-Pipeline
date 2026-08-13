namespace Triowave.Models.CustomModels
{
    public class SymbolData
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Exchange { get; set; }
        public string AssetType { get; set; }
        public DateOnly IPODate { get; set; }
        public string Status { get; set; }
    }
}
