using Microsoft.AspNetCore.SignalR;
using CurrencyTrading.Business.Models;
using System.Collections.Generic;

namespace CurrencyTrading.Business.Services
{
    public class TradingHub : Hub
    {
        // אובייקט של TradingService
        private readonly ITradingService _tradingService;

        public TradingHub(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }

        // הודעה שנשלחת מ-UI כדי לקבל את העדכונים בזמן אמת
        public async Task SendRatesUpdate(List<TradeUpdate> updates)
        {
            // שלח את העדכונים לכולם
            await Clients.All.SendAsync("RatesUpdated", updates);
        }
    }
}
