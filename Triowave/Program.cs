using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Triowave.Models;
using Triowave.Services;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddHttpClient<WebApiService>();
builder.Services.AddScoped<GeneralDWService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/MarketData/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MarketData}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
