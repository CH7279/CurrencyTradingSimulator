using CurrencyTrading.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

public class CurrencyPair
{
    public int Id { get; set; }

    public int BaseCurrencyId { get; set; }
    public int QuoteCurrencyId { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal MinValue { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal MaxValue { get; set; }

    // Navigation properties
    [ForeignKey("BaseCurrencyId")]
    public virtual Currency BaseCurrency { get; set; }

    [ForeignKey("QuoteCurrencyId")]
    public virtual Currency QuoteCurrency { get; set; }
}
