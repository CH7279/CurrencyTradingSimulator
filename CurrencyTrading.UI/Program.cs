using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using CurrencyTrading.Data.Context;
using CurrencyTrading.Business.Services;
using CurrencyTrading.Data.Repositories; // הוספת ריפוזיטוריות

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();  // הוספת SignalR לשירותים

// הוספת Entity Framework
builder.Services.AddDbContext<CurrencyTradingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// הוספת שירותי ה-DI (Dependency Injection)
builder.Services.AddScoped<ITradingService, TradingService>(); // שירות המסחר
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>(); // ריפוזיטוריית המטבעות

// הוספת תמיכה ב-Controllers עם Views (MVC)
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHubContext<TradingHub>>();  // הוספת ה-IHubContext

var app = builder.Build();

// הגדרת ה-Route של האפליקציה
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapHub<TradingHub>("/tradingHub");
// הרצת האפליקציה

app.Run();
