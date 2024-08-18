using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;
using System.Collections.Generic;
using Weather_App.Repositories;

namespace WeatherApp.Controllers
{
    [Authorize] // Requires the user to be authenticated to access any action in this controller
    public class HomeController : Controller
    {

        private readonly IMongoDBRepository _repository;

    public HomeController(IMongoDBRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var weatherData = await _repository.GetAllWeatherData();
        return View(weatherData);
    }
        public IActionResult Index2()
        {
            // Sample data
            var weatherForecasts = new List<WeatherForecast>
            {
                new WeatherForecast { Date = DateTime.Now, Temperature = 25, MainStatus = "Warm" },
                new WeatherForecast { Date = DateTime.Now.AddDays(1), Temperature = 22, MainStatus = "Cool" },
                new WeatherForecast { Date = DateTime.Now.AddDays(2), Temperature = 30, MainStatus = "Hot" }
            };

            return View(weatherForecasts);
        }
    }
}
