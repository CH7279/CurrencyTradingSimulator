using CurrencyTrading.Business.Services;
using CurrencyTrading.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyTrading.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITradingService _tradingService;

        // ����������� ����� �� ������ �� �����
        public HomeController(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }

        // ����� �� ������ �� �� ������� �� ���� �������
        public async Task<IActionResult> Index()
        {
            var rates = await _tradingService.GetCurrentRatesAsync(); // ����� ������ ��� ���� �� ���� ������� ��������
            return View(rates); // ���� �� ����� ������
        }

        // ����� ������ (Privacy) - �� ����� ����
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
