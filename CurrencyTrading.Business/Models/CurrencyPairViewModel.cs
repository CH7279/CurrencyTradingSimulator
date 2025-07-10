
namespace CurrencyTrading.Business.Models
{
    public class CurrencyPairViewModel
    {
        public int PairId { get; set; }
        public string BaseCurrencyAbbreviation { get; set; }
        public string QuoteCurrencyAbbreviation { get; set; }
        public string BaseCurrencyName { get; set; }
        public string QuoteCurrencyName { get; set; }
        public decimal CurrentRate { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsNewMin { get; set; }
        public bool IsNewMax { get; set; }
    }
}
