using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Weather_App.Models;
using Weather_App.Options;

namespace Weather_App.Services
{
    public class WeatherApiService : IWeatherApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherApiService(IOptions<WeatherApiOptions> weatherApiOptions, HttpClient httpClient)
        {
            _httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri(weatherApiOptions.Value.BaseUrl);
            _apiKey = weatherApiOptions.Value.ApiKey;
        }

        public async Task<WeatherForecast> GetCurrentWeatherByCity(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"current.json?key={_apiKey}&q={city}");
                response.EnsureSuccessStatusCode();
                var weatherData = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
                return MapToWeatherForecast(weatherData);
            }
            catch (HttpRequestException e)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while fetching current weather data: {e.Message}");
                return null;
            }
        }

        public async Task<List<WeatherForecast>> GetWeeklyWeatherForecastByCity(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"forecast.json?key={_apiKey}&q={city}&days=7");
                response.EnsureSuccessStatusCode();
                var weatherData = await response.Content.ReadFromJsonAsync<WeatherApiResponse>();
                return MapToWeeklyWeatherForecast(weatherData);
            }
            catch (HttpRequestException e)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while fetching weekly forecast data: {e.Message}");
                return new List<WeatherForecast>();
            }
        }

        private WeatherForecast MapToWeatherForecast(WeatherApiResponse response)
        {
            return new WeatherForecast
            {
                CityName = response.Location.Name,
                Date = DateTime.Now,
                Temperature = (int)response.Current.Temp_c,
                MainStatus = response.Current.Condition.Text
            };
        }

        private List<WeatherForecast> MapToWeeklyWeatherForecast(WeatherApiResponse response)
        {
            return response.Forecast.Forecastday.Select(f => new WeatherForecast
            {
                CityName = response.Location.Name,
                Date = f.Date,
                Temperature = (int)f.Day.Avgtemp_c,
                MainStatus = f.Day.Condition.Text
            }).ToList();
        }
    }
}