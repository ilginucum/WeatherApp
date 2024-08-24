using Microsoft.AspNetCore.Mvc.Rendering;
using Weather_App.Models;

namespace Weather_App.Models
{
    public class ManageWeatherDataViewModel
    {
        public IEnumerable<WeatherForecast> WeatherForecasts { get; set; }
        public SelectList Cities { get; set; }
        public string SelectedCity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}