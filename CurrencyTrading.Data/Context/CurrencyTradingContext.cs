using CurrencyTrading.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyTrading.Data.Context
{
    public class CurrencyTradingContext : DbContext
    {
        public CurrencyTradingContext(DbContextOptions<CurrencyTradingContext> options) : base(options) { }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyPair> CurrencyPairs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // הגדרת דיוק (precision) עבור שדות עשרוניים
            modelBuilder.Entity<CurrencyPair>()
                .Property(p => p.MinValue)
                .HasColumnType("decimal(18, 4)");

            modelBuilder.Entity<CurrencyPair>()
                .Property(p => p.MaxValue)
                .HasColumnType("decimal(18, 4)");

            // הגדרת מפתחות זרים ברורים
            modelBuilder.Entity<CurrencyPair>()
                .HasOne(cp => cp.BaseCurrency)
                .WithMany()
                .HasForeignKey(cp => cp.BaseCurrencyId)
                .OnDelete(DeleteBehavior.NoAction);  // הגדרת Delete Behavior

            modelBuilder.Entity<CurrencyPair>()
                .HasOne(cp => cp.QuoteCurrency)
                .WithMany()
                .HasForeignKey(cp => cp.QuoteCurrencyId)
                .OnDelete(DeleteBehavior.NoAction);  // הגדרת Delete Behavior

            // Seed data
            modelBuilder.Entity<Currency>().HasData(
                new Currency { Id = 1, Country = "United States", Name = "US Dollar", Abbreviation = "USD" },
                new Currency { Id = 2, Country = "European Union", Name = "Euro", Abbreviation = "EUR" },
                new Currency { Id = 3, Country = "United Kingdom", Name = "British Pound", Abbreviation = "GBP" },
                new Currency { Id = 4, Country = "Japan", Name = "Japanese Yen", Abbreviation = "JPY" }
            );

            modelBuilder.Entity<CurrencyPair>().HasData(
                new CurrencyPair { Id = 1, BaseCurrencyId = 1, QuoteCurrencyId = 2, MinValue = 0.8200m, MaxValue = 0.8700m },
                new CurrencyPair { Id = 2, BaseCurrencyId = 1, QuoteCurrencyId = 3, MinValue = 0.7500m, MaxValue = 0.8100m },
                new CurrencyPair { Id = 3, BaseCurrencyId = 1, QuoteCurrencyId = 4, MinValue = 145.00m, MaxValue = 155.00m }
            );
        }



    }
}
