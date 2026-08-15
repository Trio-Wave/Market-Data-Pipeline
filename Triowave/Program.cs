using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Triowave.Interfaces;
using Triowave.Models;
using Triowave.Services;

var builder = WebApplication.CreateBuilder(args);

// Host as Windows Service
builder.Host.UseWindowsService();

// Connection string
var connectionString =
    builder.Configuration.GetConnectionString("GeneralDW")
    ?? throw new InvalidOperationException(
        "Connection string 'GeneralDW' not found.");

// Entity Framework
builder.Services.AddDbContext<GeneralDWContext>(options =>
    options.UseSqlServer(connectionString));

// Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));

builder.Services.AddHangfireServer();

// MVC
builder.Services.AddControllersWithViews();

// Application services
builder.Services.AddSingleton<AppSettingsService>();
builder.Services.AddHttpClient<IWebApiService, WebApiService>();
builder.Services.AddScoped<IGeneralDWService, GeneralDWService>();
builder.Services.AddScoped<IMarketDataService, MarketDataService>();

// Logging
builder.Host.UseSerilog(
    (context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Logging
app.UseSerilogRequestLogging();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("===== APPLICATION STARTED =====");

// Hangfire Dashboard
app.UseHangfireDashboard();

// Hangfire recurring jobs
RecurringJob.AddOrUpdate<IMarketDataService>(
    "daily-stock-quotes",
    service => service.PullDailyStockQuote(),
    Cron.Daily(20),
    new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")
    });

// HTTP pipeline
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MarketData}/{action=Index}/{id?}")
    .WithStaticAssets();

// app.UseHttpsRedirection();

app.Run();