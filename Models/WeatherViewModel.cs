namespace Weather_App.Models
{
    public class WeatherViewModel
    {
        public WeatherForecast CurrentWeather { get; set; }
        public List<WeatherForecast> WeeklyForecast { get; set; }
        public string SearchCity { get; set; }
    }
}