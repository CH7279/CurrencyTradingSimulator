
using Microsoft.AspNetCore.Mvc;
using CurrencyTrading.Business.Services;

namespace CurrencyTrading.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITradingService _tradingService;

        public HomeController(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Start simulation if not running
                if (!_tradingService.IsRunning)
                {
                    await _tradingService.StartSimulationAsync();
                }

                var currentRates = await _tradingService.GetCurrentRatesViewAsync();
                return View(currentRates);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<CurrencyTrading.Business.Models.CurrencyPairViewModel>());
            }
        }
    }
}
