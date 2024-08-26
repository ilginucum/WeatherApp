using System.Collections.Generic;
using System.Threading.Tasks;
using Weather_App.Models;

namespace Weather_App.Repositories
{
    public interface IMongoDBRepository
    {
        Task<IEnumerable<UserRegistration>> GetAllUsers();
        Task<UserRegistration> AddUser(UserRegistration user);
        Task DeleteUser(string username);
        Task<IEnumerable<WeatherForecast>> GetAllWeatherData();
        Task AddWeatherData(WeatherForecast weatherData);
        Task DeleteWeatherData(string id);

        Task SaveUserRegistration(UserRegistration userRegistration);
        Task SaveUserLogin(UserLogin userLogin);
        Task<UserRegistration> GetUserByUsername(string username);
        Task LogUserLogin(UserLogin userLogin);
        Task<IEnumerable<UserLogin>> GetUserLogins(string username);
        Task UpdateUser(string originalUsername, UserRegistration updatedUser);
        Task InitializeUserTypeIfNeeded(string username);
        Task<WeatherForecast> GetTodayWeatherDataByCity(string cityName);
        Task<List<WeatherForecast>> GetWeeklyWeatherForecastByCity(string city);
        Task EditUser(UserRegistration user);
        Task IncrementFailedLoginAttempts(string username);
        Task ResetFailedLoginAttempts(string username);
        Task<bool> IsUserLockedOut(string username);


       
        
        
        
    }
}