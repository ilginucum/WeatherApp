using Weather_App.Models;
using Weather_App.Options;

namespace Weather_App.Services
{
    public interface IWeatherApiService
    {
        Task<WeatherForecast> GetCurrentWeatherByCity(string city);
        Task<List<WeatherForecast>> GetWeeklyWeatherForecastByCity(string city);
    }
}