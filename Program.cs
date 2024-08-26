using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather_App.Models;
using Weather_App.Options;
using Weather_App.Repositories;
using Weather_App.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

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
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure WeatherAPI options
builder.Services.Configure<WeatherApiOptions>(builder.Configuration.GetSection("WeatherApiOptions"));
// Add HttpClient and WeatherApiService
builder.Services.AddHttpClient<IWeatherApiService, WeatherApiService>((serviceProvider, client) =>
{
    var weatherApiOptions = serviceProvider.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
    if (string.IsNullOrEmpty(weatherApiOptions.BaseUrl))
    {
        throw new InvalidOperationException("WeatherApiOptions.BaseUrl is null or empty. Check your configuration.");
    }
    client.BaseAddress = new Uri(weatherApiOptions.BaseUrl);
});

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
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();