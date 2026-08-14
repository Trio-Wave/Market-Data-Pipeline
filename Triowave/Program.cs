using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Triowave.Interfaces;
using Triowave.Models;
using Triowave.Services;

var builder = WebApplication.CreateBuilder(args);

// Host as windows service
builder.Host.UseWindowsService();

// Add Hangfire services
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("GeneralDW")));

builder.Services.AddHangfireServer();


// Add database connection
var connectionString =
    builder.Configuration.GetConnectionString("GeneralDW")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

builder.Services.AddDbContext<GeneralDWContext>(options =>
    options.UseSqlServer(connectionString));

using (SqlConnection conn = new SqlConnection(connectionString))
{
    try
    {
        conn.Open();
        Console.WriteLine("Connection successful!");
    }
    catch (SqlException ex)
    {
        Console.WriteLine("SQL Error: " + ex.Message);
    }
}

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<AppSettingsService>();
builder.Services.AddHttpClient<IWebApiService, WebApiService>();
builder.Services.AddScoped<IGeneralDWService, GeneralDWService>();
builder.Services.AddScoped<IMarketDataService, MarketDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/MarketData/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

# region Hangfire

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IMarketDataService>(
    "daily-stock-quotes",
    service => service.PullDailyStockQuote(),
    Cron.Daily(20));

#endregion

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MarketData}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
