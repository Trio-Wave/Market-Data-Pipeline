using Triowave.Models.CustomModels;

namespace Triowave.Models.ViewModels
{
    public class MarketDataViewModel
    {
        public IQueryable<GlobalQuote> GlobalQuotes { get; set; } = default!;

        public List<string> Symbols { get; set; } = new();

        public string SelectedSymbol { get; set; } = "";
    }
}
