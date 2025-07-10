using CurrencyTrading.Business.Models;

namespace CurrencyTrading.Business.Services
{
    public interface ITradingService
    {
        Task<List<TradeUpdate>> GetCurrentRatesAsync();
        Task<List<CurrencyPairViewModel>> GetCurrentRatesViewAsync();
        Task StartSimulationAsync();
        Task StopSimulationAsync();
        bool IsRunning { get; }
        event EventHandler<List<TradeUpdate>> RatesUpdated;
    }
}
