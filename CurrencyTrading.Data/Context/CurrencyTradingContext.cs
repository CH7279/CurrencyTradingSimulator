using Microsoft.EntityFrameworkCore;
using CurrencyTrading.Data.Models;

namespace CurrencyTrading.Data.Context
{
    public class CurrencyTradingContext : DbContext
    {
        public CurrencyTradingContext(DbContextOptions<CurrencyTradingContext> options) : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyPair> CurrencyPairs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<CurrencyPair>()
                .HasOne(cp => cp.BaseCurrency)
                .WithMany(c => c.BaseCurrencyPairs)
                .HasForeignKey(cp => cp.BaseCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CurrencyPair>()
                .HasOne(cp => cp.QuoteCurrency)
                .WithMany(c => c.QuoteCurrencyPairs)
                .HasForeignKey(cp => cp.QuoteCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed currencies
            modelBuilder.Entity<Currency>().HasData(
                new Currency { Id = 1, Country = "United States", Name = "US Dollar", Abbreviation = "USD" },
                new Currency { Id = 2, Country = "European Union", Name = "Euro", Abbreviation = "EUR" },
                new Currency { Id = 3, Country = "United Kingdom", Name = "British Pound", Abbreviation = "GBP" },
                new Currency { Id = 4, Country = "Japan", Name = "Japanese Yen", Abbreviation = "JPY" }
            );

            // Seed currency pairs
            modelBuilder.Entity<CurrencyPair>().HasData(
                new CurrencyPair { Id = 1, BaseCurrencyId = 1, QuoteCurrencyId = 2, MinValue = 0.8200m, MaxValue = 0.8700m },
                new CurrencyPair { Id = 2, BaseCurrencyId = 1, QuoteCurrencyId = 3, MinValue = 0.7500m, MaxValue = 0.8100m },
                new CurrencyPair { Id = 3, BaseCurrencyId = 1, QuoteCurrencyId = 4, MinValue = 145.00m, MaxValue = 155.00m }
            );
        }
    }
}
