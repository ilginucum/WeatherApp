using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using Weather_App.Models;
using Weather_App.Options;

namespace Weather_App.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly IOptions<WeatherApiOptions> _weatherApiOptions;
        private readonly HttpClient _httpClient;

        public WeatherApiService(IOptions<WeatherApiOptions> weatherApiOptions, HttpClient httpClient)
        {
            _weatherApiOptions = weatherApiOptions;
            _httpClient = httpClient;
        }

        public async Task<WeatherForecast> GetCurrentWeatherByCity(string city)
        {
            var apiUrl = $"{_weatherApiOptions.Value.BaseUrl}?q={city}&appid={_weatherApiOptions.Value.ApiKey}&units=metric";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadFromJsonAsync<OpenWeatherMapResponse>();
                return MapToWeatherForecast(weatherData);
            }

            return null;
        }

        public async Task<List<WeatherForecast>> GetWeeklyWeatherForecastByCity(string city)
        {
            var apiUrl = $"{_weatherApiOptions.Value.BaseUrl}?q={city}&appid={_weatherApiOptions.Value.ApiKey}&units=metric&cnt=7&mode=json";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var weatherData = await response.Content.ReadFromJsonAsync<OpenWeatherMapForecastResponse>();
                if (weatherData?.List != null)
                {
                    var weeklyForecast = weatherData.List.Select(item => MapToWeeklyWeatherForecast(item)).ToList();
                    foreach (var forecast in weeklyForecast)
                    {
                        Console.WriteLine($"Date: {forecast.Date}, Temperature: {forecast.Temperature}, Status: {forecast.MainStatus}");
                    }
                    return weeklyForecast;
                }
            }

            return new List<WeatherForecast>();
        }

        private WeatherForecast MapToWeatherForecast(OpenWeatherMapResponse response)
        {
            return new WeatherForecast
            {
                CityName = response.Name,
                Date = DateTime.Now,
                Temperature = (int)response.Main.Temp,
                MainStatus = response.Weather[0].Main
            };
        }

        private WeatherForecast MapToWeeklyWeatherForecast(OpenWeatherMapForecastItem item)
        {
            return new WeatherForecast
            {
                CityName = item.Name,
                Date = item.DtTxt,
                Temperature = (int)item.Main.Temp,
                MainStatus = item.Weather[0].Main
            };
        }
    }
}