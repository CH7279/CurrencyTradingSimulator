using CurrencyTrading.Business.Services;
using CurrencyTrading.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTrading.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITradingService _tradingService;

        // קונסטרוקטור המקבל את השירות של המסחר
        public HomeController(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }

        // פעולה זו מחזירה את כל הנתונים של שערי המטבעות
        public async Task<IActionResult> Index()
        {
            var rates = await _tradingService.GetCurrentRatesAsync(); // קריאה לשירות כדי לקבל את שערי המטבעות הנוכחיים
            return View(rates); // שולח את המידע לתצוגה
        }

        // פעולת פרטיות (Privacy) - לא משמשת כרגע
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
