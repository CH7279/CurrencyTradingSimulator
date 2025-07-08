using CurrencyTrading.Data.Repositories;
using CurrencyTrading.Data.Models;
using CurrencyTrading.Business.Models;
using System.Threading;

namespace CurrencyTrading.Business.Services
{
    public class TradingService : ITradingService, IDisposable
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly Timer _timer;
        private readonly Random _random;
        private List<CurrencyPair> _currencyPairs;
        private Dictionary<int, decimal> _currentRates;
        private Dictionary<int, decimal> _previousRates;

        public event EventHandler<List<TradeUpdate>> RatesUpdated;

        public TradingService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
            _timer = new Timer(OnTimerElapsed, null, 0, 2000); // 2000ms == 2 seconds
            _random = new Random();
            _currentRates = new Dictionary<int, decimal>();
            _previousRates = new Dictionary<int, decimal>();
        }

        // פונקציה לקבלת השערים הנוכחיים
        public async Task<List<TradeUpdate>> GetCurrentRatesAsync()
        {
            if (_currencyPairs == null)
            {
                _currencyPairs = await _currencyRepository.GetAllCurrencyPairsAsync();
                InitializeRates();
            }

            return _currencyPairs.Select(pair => new TradeUpdate
            {
                PairId = pair.Id,
                NewRate = _currentRates[pair.Id],
                Change = _currentRates[pair.Id] - _previousRates[pair.Id],
                ChangePercent = ((_currentRates[pair.Id] - _previousRates[pair.Id]) / _previousRates[pair.Id]) * 100,
                Timestamp = DateTime.Now
            }).ToList();
        }

        // הפעלת סימולציה
        public async Task StartSimulationAsync()
        {
            if (_currencyPairs == null)
            {
                _currencyPairs = await _currencyRepository.GetAllCurrencyPairsAsync();
                InitializeRates();
            }
            _timer.Change(0, 2000); // 2000ms == 2 seconds
        }

        // עצירת סימולציה
        public Task StopSimulationAsync()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite); // Stops the timer
            return Task.CompletedTask;
        }

        // אתחול שערים
        private void InitializeRates()
        {
            foreach (var pair in _currencyPairs)
            {
                var initialRate = (pair.MinValue + pair.MaxValue) / 2;
                _currentRates[pair.Id] = initialRate;
                _previousRates[pair.Id] = initialRate;
            }
        }

        // עדכון שערים בתום כל טיימר
        private async void OnTimerElapsed(object state)
        {
            if (_currencyPairs == null) return;
            var updates = new List<TradeUpdate>();

            foreach (var pair in _currencyPairs)
            {
                if (pair == null) continue; // בדוק אם pair הוא null

                // עדכון שערים קודמים
                _previousRates[pair.Id] = _currentRates[pair.Id];

                // 2% תנודתיות
                var volatility = 0.02m;  // volatility הוא decimal
                var change = ((decimal)(_random.NextDouble()) - 0.5m) * 2 * volatility; // המרת _random.NextDouble() ל-decimal
                var newRate = _currentRates[pair.Id] * (1 + change);  // חישוב שער חדש


                // ודא שהשער לא יפול לאזור קיצוני מדי
                newRate = Math.Max(newRate, _currentRates[pair.Id] * 0.5m);

                _currentRates[pair.Id] = newRate;

                // בדוק אם יש שער מינימום ומקסימום חדש
                bool isNewMin = newRate < pair.MinValue;
                bool isNewMax = newRate > pair.MaxValue;

                if (isNewMin || isNewMax)
                {
                    var newMinValue = Math.Min(pair.MinValue, newRate);
                    var newMaxValue = Math.Max(pair.MaxValue, newRate);
                    await _currencyRepository.UpdateCurrencyPairMinMaxAsync(pair.Id, newMinValue, newMaxValue);

                    // עדכון המטמון המקומי
                    pair.MinValue = newMinValue;
                    pair.MaxValue = newMaxValue;
                }

                // הוסף את העדכון
                updates.Add(new TradeUpdate
                {
                    PairId = pair.Id,
                    NewRate = newRate,
                    Change = newRate - _previousRates[pair.Id],
                    ChangePercent = ((newRate - _previousRates[pair.Id]) / _previousRates[pair.Id]) * 100,
                    Timestamp = DateTime.Now,
                    IsNewMin = isNewMin,
                    IsNewMax = isNewMax
                });
            }

            // קריאה לאירוע אם יש מאזינים
            RatesUpdated?.Invoke(this, updates);
        }

        // שחרור משאבים
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
