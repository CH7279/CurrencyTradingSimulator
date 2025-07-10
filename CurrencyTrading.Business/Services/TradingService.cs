
using CurrencyTrading.Business.Models;
using CurrencyTrading.Data.Repositories;
using CurrencyTrading.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Timers;

namespace CurrencyTrading.Business.Services
{
    public class TradingService : ITradingService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TradingService> _logger;
        private readonly System.Timers.Timer _timer;
        private readonly Random _random;
        private List<CurrencyPair> _currencyPairs;
        private Dictionary<int, decimal> _currentRates;
        private Dictionary<int, decimal> _previousRates;
        private bool _isRunning;

        public event EventHandler<List<TradeUpdate>> RatesUpdated;

        public TradingService(IServiceProvider serviceProvider, ILogger<TradingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _timer = new System.Timers.Timer(2000); // 2 seconds
            _timer.Elapsed += OnTimerElapsed;
            _random = new Random();
            _currentRates = new Dictionary<int, decimal>();
            _previousRates = new Dictionary<int, decimal>();
            _isRunning = false;
        }

        public bool IsRunning => _isRunning;

        public async Task<List<CurrencyPairViewModel>> GetCurrentRatesViewAsync()
        {
            if (_currencyPairs == null)
            {
                await InitializeAsync();
            }

            return _currencyPairs.Select(pair => new CurrencyPairViewModel
            {
                PairId = pair.Id,
                BaseCurrencyAbbreviation = pair.BaseCurrency.Abbreviation,
                QuoteCurrencyAbbreviation = pair.QuoteCurrency.Abbreviation,
                BaseCurrencyName = pair.BaseCurrency.Name,
                QuoteCurrencyName = pair.QuoteCurrency.Name,
                CurrentRate = _currentRates.GetValueOrDefault(pair.Id, 0),
                Change = _currentRates.GetValueOrDefault(pair.Id, 0) - _previousRates.GetValueOrDefault(pair.Id, 0),
                ChangePercent = _previousRates.GetValueOrDefault(pair.Id, 0) != 0 ?
                    ((_currentRates.GetValueOrDefault(pair.Id, 0) - _previousRates.GetValueOrDefault(pair.Id, 0)) / _previousRates.GetValueOrDefault(pair.Id, 0)) * 100 : 0,
                MinValue = pair.MinValue,
                MaxValue = pair.MaxValue,
                LastUpdate = DateTime.Now
            }).ToList();
        }

        public async Task<List<TradeUpdate>> GetCurrentRatesAsync()
        {
            if (_currencyPairs == null)
            {
                await InitializeAsync();
            }

            return _currencyPairs.Select(pair => new TradeUpdate
            {
                PairId = pair.Id,
                NewRate = _currentRates.GetValueOrDefault(pair.Id, 0),
                Change = _currentRates.GetValueOrDefault(pair.Id, 0) - _previousRates.GetValueOrDefault(pair.Id, 0),
                ChangePercent = _previousRates.GetValueOrDefault(pair.Id, 0) != 0 ?
                    ((_currentRates.GetValueOrDefault(pair.Id, 0) - _previousRates.GetValueOrDefault(pair.Id, 0)) / _previousRates.GetValueOrDefault(pair.Id, 0)) * 100 : 0,
                Timestamp = DateTime.Now
            }).ToList();
        }

        public Task StartSimulationAsync()
        {
            _isRunning = true;
            _timer.Start();
            _logger.LogInformation("Trading simulation started");
            return Task.CompletedTask;
        }

        public Task StopSimulationAsync()
        {
            _isRunning = false;
            _timer.Stop();
            _logger.LogInformation("Trading simulation stopped");
            return Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

            _currencyPairs = await repository.GetAllCurrencyPairsAsync();

            foreach (var pair in _currencyPairs)
            {
                var initialRate = (pair.MinValue + pair.MaxValue) / 2;
                _currentRates[pair.Id] = initialRate;
                _previousRates[pair.Id] = initialRate;
            }

            _logger.LogInformation($"Initialized {_currencyPairs.Count} currency pairs");
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (!_isRunning || _currencyPairs == null) return;

            var updates = new List<TradeUpdate>();
            var hasChanges = false;

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();

            foreach (var pair in _currencyPairs)
            {
                // Save previous rate
                _previousRates[pair.Id] = _currentRates[pair.Id];

                // Generate new rate with 2% volatility
                var volatility = 0.02;
                var change = (_random.NextDouble() - 0.5) * 2 * volatility;
                var newRate = _currentRates[pair.Id] * (1 + (decimal)change);

                // Ensure rate doesn't go negative or too extreme
                newRate = Math.Max(newRate, 0.0001m);
                newRate = Math.Max(newRate, _currentRates[pair.Id] * 0.98m);
                newRate = Math.Min(newRate, _currentRates[pair.Id] * 1.02m);

                _currentRates[pair.Id] = newRate;

                // Check for new min/max and update DB
                bool isNewMin = newRate < pair.MinValue;
                bool isNewMax = newRate > pair.MaxValue;

                if (isNewMin || isNewMax)
                {
                    var newMinValue = Math.Min(pair.MinValue, newRate);
                    var newMaxValue = Math.Max(pair.MaxValue, newRate);

                    // Update database
                    await repository.UpdateCurrencyPairMinMaxAsync(pair.Id, newMinValue, newMaxValue);

                    // Update local cache
                    pair.MinValue = newMinValue;
                    pair.MaxValue = newMaxValue;

                    hasChanges = true;
                    _logger.LogInformation($"Updated min/max for {pair.BaseCurrency.Abbreviation}/{pair.QuoteCurrency.Abbreviation}: Min={newMinValue:F4}, Max={newMaxValue:F4}");
                }

                updates.Add(new TradeUpdate
                {
                    PairId = pair.Id,
                    NewRate = newRate,
                    Change = newRate - _previousRates[pair.Id],
                    ChangePercent = _previousRates[pair.Id] != 0 ? ((newRate - _previousRates[pair.Id]) / _previousRates[pair.Id]) * 100 : 0,
                    Timestamp = DateTime.Now,
                    IsNewMin = isNewMin,
                    IsNewMax = isNewMax
                });
            }

            _logger.LogInformation($"Generated {updates.Count} rate updates. DB changes: {hasChanges}");
            RatesUpdated?.Invoke(this, updates);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
