using System.Collections.Generic;
using System.Threading.Tasks;
using Weather_App.Models;
using Weather_App.Controllers;

namespace Weather_App.Repositories
{
    public interface IMongoDBRepository
    {
        Task<IEnumerable<UserLogin>> GetAllUsers();
        Task<UserLogin> AddUser(UserLogin user);
        Task DeleteUser(string id);
        Task<IEnumerable<WeatherForecast>> GetAllWeatherData();
        Task<WeatherForecast> AddWeatherData(WeatherForecast weatherData);
        Task DeleteWeatherData(string id);
    }
}