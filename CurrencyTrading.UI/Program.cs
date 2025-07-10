
using CurrencyTrading.Business.Services;
using CurrencyTrading.Data.Context;
using Microsoft.EntityFrameworkCore;
using CurrencyTrading.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework
builder.Services.AddDbContext<CurrencyTradingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add SignalR
builder.Services.AddSignalR();

// Add DI services
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddSingleton<ITradingService, TradingService>();

// Add Controllers with Views (MVC)
builder.Services.AddControllersWithViews();

// Add API Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Configure routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map API controllers
app.MapControllers();


// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CurrencyTradingContext>();
    context.Database.EnsureCreated();
}

app.Run();
