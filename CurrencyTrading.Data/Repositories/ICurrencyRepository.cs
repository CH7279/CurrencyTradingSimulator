using CurrencyTrading.Data.Models;

namespace CurrencyTrading.Data.Repositories
{
    public interface ICurrencyRepository
    {
        Task<List<CurrencyPair>> GetAllCurrencyPairsAsync();
        Task UpdateCurrencyPairMinMaxAsync(int pairId, decimal newMinValue, decimal newMaxValue);
    }
}
