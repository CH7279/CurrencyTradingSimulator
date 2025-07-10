using Microsoft.AspNetCore.Mvc;
using CurrencyTrading.Business.Services;
using CurrencyTrading.Business.Models;

namespace CurrencyTrading.UI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ITradingService _tradingService;

        public ApiController(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }

        [HttpGet("rates")]
        public async Task<ActionResult<List<CurrencyPairViewModel>>> GetRates()
        {
            //print("GetRates called");
            Console.WriteLine("GetRates called");
            try
            {
                var rates = await _tradingService.GetCurrentRatesViewAsync();
                //print($"GetRates returned {rates.Count} pairs");
                if (rates == null || rates.Count == 0)
                {
                    return NotFound(new { message = "No currency pairs found" });
                }
                //print("Returning rates");
                // Log the rates for debugging purposes
                foreach (var rate in rates)
                {
                    //print($"Rate: {rate.BaseCurrencyAbbreviation}/{rate.QuoteCurrencyAbbreviation} - {rate.CurrentRate}");
                    Console.WriteLine($"Rate: {rate.BaseCurrencyAbbreviation}/{rate.QuoteCurrencyAbbreviation} - {rate.CurrentRate}");
                }
                return Ok(rates);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSimulation()
        {
            try
            {
                await _tradingService.StartSimulationAsync();
                return Ok(new { message = "Simulation started" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopSimulation()
        {
            try
            {
                await _tradingService.StopSimulationAsync();
                return Ok(new { message = "Simulation stopped" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
