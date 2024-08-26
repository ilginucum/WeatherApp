using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;
using Weather_App.Repositories;
using Weather_App.Services;
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
        public async Task<IActionResult> Index(string searchCity = null)
        {
            var username = User.Identity.Name;
            var user = await _repository.GetUserByUsername(username);

            string cityToUse = searchCity ?? user?.DefaultCity;

            if (string.IsNullOrEmpty(cityToUse))
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

            var currentWeather = await _repository.GetTodayWeatherDataByCity(cityToUse);
            if (currentWeather == null)
            {
                currentWeather = await _weatherApiService.GetCurrentWeatherByCity(cityToUse);
                if (currentWeather != null)
                {
                    await _repository.AddWeatherData(currentWeather);
                }
            }

            if (currentWeather == null)
            {
                currentWeather = new WeatherForecast
                {
                    CityName = cityToUse,
                    Date = DateTime.Now,
                    Temperature = 0,
                    MainStatus = "No data available"
                };
            }

            var weeklyForecast = await _weatherApiService.GetWeeklyWeatherForecastByCity(cityToUse);

            var viewModel = new WeatherViewModel
            {
                CurrentWeather = currentWeather,
                WeeklyForecast = weeklyForecast,
                SearchCity = searchCity
            };

            return View(viewModel);
        }
    }
}



