using System.Collections.Generic;
using System.Threading.Tasks;
using Weather_App.Models;

namespace Weather_App.Repositories
{
    public interface IMongoDBRepository
    {
        Task<IEnumerable<UserLogin>> GetAllUsers();
        Task<UserLogin> AddUser(UserLogin user);
        Task DeleteUser(string id);
        Task<IEnumerable<WeatherForecast>> GetAllWeatherData();
        Task AddWeatherData(WeatherForecast weatherData);
        Task DeleteWeatherData(string id);

        Task SaveUserRegistration(UserRegistration userRegistration);
        Task SaveUserLogin(UserLogin userLogin);
        Task<UserRegistration> GetUserByUsername(string username);
        Task LogUserLogin(UserLogin userLogin);
        Task<IEnumerable<UserLogin>> GetUserLogins(string username);
        Task UpdateUser(UserRegistration updatedUser);
        Task InitializeUserTypeIfNeeded(string username);
    }
}