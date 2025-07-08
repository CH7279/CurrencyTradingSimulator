using Microsoft.EntityFrameworkCore;
using CurrencyTrading.Data.Context;
using CurrencyTrading.Data.Models;

namespace CurrencyTrading.Data.Repositories
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly CurrencyTradingContext _context;

        public CurrencyRepository(CurrencyTradingContext context)
        {
            _context = context;
        }

        public async Task<List<CurrencyPair>> GetAllCurrencyPairsAsync()
        {
            return await _context.CurrencyPairs
                .Include(cp => cp.BaseCurrency)
                .Include(cp => cp.QuoteCurrency)
                .ToListAsync();
        }

        public async Task UpdateCurrencyPairMinMaxAsync(int pairId, decimal newMinValue, decimal newMaxValue)
        {
            var pair = await _context.CurrencyPairs.FindAsync(pairId);
            if (pair != null)
            {
                pair.MinValue = newMinValue;
                pair.MaxValue = newMaxValue;
                await _context.SaveChangesAsync();
            }
        }
    }
}
