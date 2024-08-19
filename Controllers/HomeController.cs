using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;
using Weather_App.Repositories;

namespace WeatherApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IMongoDBRepository _repository;

        public HomeController(IMongoDBRepository repository)
        {
            _repository = repository;
        }

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
                currentWeather = new WeatherForecast
                {
                    CityName = user.DefaultCity,
                    Date = DateTime.Now,
                    Temperature = 0,
                    MainStatus = "No data available"
                };
            }

            var weeklyForecast = await _repository.GetWeeklyWeatherForecastByCity(user.DefaultCity);

            var viewModel = new WeatherViewModel
            {
                CurrentWeather = currentWeather,
                WeeklyForecast = weeklyForecast
            };

            return View(viewModel);
        }
    }
}
