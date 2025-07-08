using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using CurrencyTrading.Data.Context;
using CurrencyTrading.Business.Services;
using CurrencyTrading.Data.Repositories; // ����� ������������

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();  // ����� SignalR ��������

// ����� Entity Framework
builder.Services.AddDbContext<CurrencyTradingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ����� ������ �-DI (Dependency Injection)
builder.Services.AddScoped<ITradingService, TradingService>(); // ����� �����
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>(); // ������������ �������

// ����� ����� �-Controllers �� Views (MVC)
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHubContext<TradingHub>>();  // ����� �-IHubContext

var app = builder.Build();

// ����� �-Route �� ���������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapHub<TradingHub>("/tradingHub");
// ���� ���������

app.Run();
