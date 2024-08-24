using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather_App.Models;
using Weather_App.Options;
using Weather_App.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using Newtonsoft.Json;
using Weather_App.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the MongoDB connection
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.AddSingleton<IMongoClient>(CreateMongoClient);
builder.Services.AddScoped<IMongoDBRepository, MongoDbRepository>();

// Add authentication services with cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirect to login page
    options.LogoutPath = "/Account/Logout"; // Add logout path
    options.AccessDeniedPath = "/Account/AccessDenied"; // Add access denied path
});

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpClient service for making API requests
builder.Services.AddHttpClient();

// Add WeatherApiService
builder.Services.Configure<WeatherApiOptions>(builder.Configuration.GetSection("WeatherApiOptions"));
builder.Services.AddHttpClient<IWeatherApiService, WeatherApiService>();

IMongoClient CreateMongoClient(IServiceProvider sp)
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>();
    return new MongoClient(settings.Value.ConnectionString);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Enable authentication
app.UseAuthorization();
app.UseSession(); // Enable session

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();