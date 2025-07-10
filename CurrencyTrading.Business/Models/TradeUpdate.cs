
namespace CurrencyTrading.Business.Models
{
    public class TradeUpdate
    {
        public int PairId { get; set; }
        public decimal NewRate { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsNewMin { get; set; }
        public bool IsNewMax { get; set; }
    }
}
