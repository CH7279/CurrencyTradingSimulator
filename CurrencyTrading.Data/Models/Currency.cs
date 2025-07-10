
using System.ComponentModel.DataAnnotations;

namespace CurrencyTrading.Data.Models
{
    public class Currency
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(10)]
        public string Abbreviation { get; set; }

        // Navigation properties
        public virtual ICollection<CurrencyPair> BaseCurrencyPairs { get; set; }
        public virtual ICollection<CurrencyPair> QuoteCurrencyPairs { get; set; }
    }
}
