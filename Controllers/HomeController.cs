using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;
using Weather_App.Repositories;
using Weather_App.Services;
using Weather_App.Options;
using System.Threading.Tasks;

namespace WeatherApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMongoDBRepository _repository;
        private readonly IWeatherApiService _weatherApiService;

        public HomeController(IMongoDBRepository repository, IWeatherApiService weatherApiService)
        {
            _repository = repository;
            _weatherApiService = weatherApiService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            var user = await _repository.GetUserByUsername(username);

            if (user == null || string.IsNullOrEmpty(user.DefaultCity))
            {
                return View(new WeatherViewModel
                {
                    CurrentWeather = new WeatherForecast
                    {
                        CityName = "Unknown",
                        Date = DateTime.Now,
                        Temperature = 0,
                        MainStatus = "User or default city not found"
                    },
                    WeeklyForecast = new List<WeatherForecast>()
                });
            }

            var currentWeather = await _repository.GetTodayWeatherDataByCity(user.DefaultCity);
            if (currentWeather == null)
            {
                // Fetch the current weather data from the Weather API
                currentWeather = await _weatherApiService.GetCurrentWeatherByCity(user.DefaultCity);
                if (currentWeather != null)
                {
                    // Save the fetched weather data to the database
                    await _repository.AddWeatherData(currentWeather);
                }
            }

            if (currentWeather == null)
            {
                currentWeather = new WeatherForecast
                {
                    CityName = user.DefaultCity,
                    Date = DateTime.Now,
                    Temperature = 0,
                    MainStatus = "No data available"
                };
            }

            // Fetch the weekly weather forecast from the Weather API
            var weeklyForecast = await _weatherApiService.GetWeeklyWeatherForecastByCity(user.DefaultCity);

            var viewModel = new WeatherViewModel
            {
                CurrentWeather = currentWeather,
                WeeklyForecast = weeklyForecast
            };

            return View(viewModel);
        }
    }
}



