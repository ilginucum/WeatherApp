using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;
using System.Collections.Generic;

namespace WeatherApp.Controllers
{
    [Authorize] // Requires the user to be authenticated to access any action in this controller
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Sample data
            var weatherForecasts = new List<WeatherForecast>
            {
                new WeatherForecast { Date = DateTime.Now, TemperatureC = 25, Summary = "Warm" },
                new WeatherForecast { Date = DateTime.Now.AddDays(1), TemperatureC = 22, Summary = "Cool" },
                new WeatherForecast { Date = DateTime.Now.AddDays(2), TemperatureC = 30, Summary = "Hot" }
            };

            return View(weatherForecasts);
        }
    }
}
