using CurrencyTrading.Business.Models;

namespace CurrencyTrading.Business.Services
{
    public interface ITradingService
    {
        Task<List<TradeUpdate>> GetCurrentRatesAsync();
        Task StartSimulationAsync();
        Task StopSimulationAsync();
        event EventHandler<List<TradeUpdate>> RatesUpdated;
    }
}
