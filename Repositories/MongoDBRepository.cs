using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather_App.Models;
using Weather_App.Options;
using System.Threading.Tasks;

namespace Weather_App.Repositories
{
    public class MongoDbRepository : IMongoDBRepository
    {
        private readonly IMongoDatabase _database;

        public MongoDbRepository(IOptions<MongoDbSettings> mongoDbSettings, IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        }

        public async Task SaveUserRegistration(UserRegistration userRegistration)
        {
            var collection = _database.GetCollection<UserRegistration>("UserRegistrations");
            await collection.InsertOneAsync(userRegistration);
        }

        public async Task SaveUserLogin(UserLogin userLogin)
        {
            var collection = _database.GetCollection<UserLogin>("UserLogins");
            await collection.InsertOneAsync(userLogin);
        }

        public async Task<UserRegistration> GetUserByUsername(string username)
        {
            var collection = _database.GetCollection<UserRegistration>("UserRegistrations");
            var user = await collection.Find(u => u.Username == username).FirstOrDefaultAsync();
            return user;
        }

         // UserLogin methods
        public async Task LogUserLogin(UserLogin userLogin)
        {
            var collection = _database.GetCollection<UserLogin>("UserLogins");
            await collection.InsertOneAsync(userLogin);
        }

        public async Task<IEnumerable<UserLogin>> GetUserLogins(string username)
        {
            var collection = _database.GetCollection<UserLogin>("UserLogins");
            return await collection.Find(u => u.Username == username).ToListAsync();
        }

        public async Task UpdateUser(UserRegistration updatedUser)
    {
        var collection = _database.GetCollection<UserRegistration>("UserRegistrations");
        var filter = Builders<UserRegistration>.Filter.Eq(u => u.Username, updatedUser.Username);
        var updateDefinition = Builders<UserRegistration>.Update
            .Set(u => u.Username, updatedUser.Username);

        if (!string.IsNullOrEmpty(updatedUser.DefaultCity))
        {
            updateDefinition = updateDefinition.Set(u => u.DefaultCity, updatedUser.DefaultCity);
        }

        if (!string.IsNullOrEmpty(updatedUser.Password))
        {
            updateDefinition = updateDefinition
                .Set(u => u.Password, updatedUser.Password)
                .Set(u => u.Salt, updatedUser.Salt);  // Add this line to update the salt
        }

        await collection.UpdateOneAsync(filter, updateDefinition);
    }


        public async Task InitializeUserTypeIfNeeded(string username)
        {
            var collection = _database.GetCollection<UserRegistration>("UserRegistrations");
            
            // Find the user by username
            var user = await collection.Find(u => u.Username == username).FirstOrDefaultAsync();
            
            // Update the userType if needed
            if (user != null && user.UserType == null) // Assuming userType should be initialized if it's null
            {
                var update = Builders<UserRegistration>.Update.Set(u => u.UserType, "lastUserType");
                await collection.UpdateOneAsync(u => u.Username == username, update);
            }
        }
        //interface repo // Implement the methods from IMongoDBRepository
    public async Task<IEnumerable<UserRegistration>> GetAllUsers()
    {
        var collection = _database.GetCollection<UserRegistration>("UserRegistrations");
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task<UserRegistration> AddUser(UserRegistration user)
    {
        var collection = _database.GetCollection<UserRegistration>("UserRegistrations");
        await collection.InsertOneAsync(user);
        return user;
    }

    public async Task DeleteUser(string username)
    {
        var collection = _database.GetCollection<UserRegistration>("UserRegistrations");
        await collection.DeleteOneAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<WeatherForecast>> GetAllWeatherData()
    {
        var collection = _database.GetCollection<WeatherForecast>("WeatherForecasts");
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task AddWeatherData(WeatherForecast weatherData)
    {
        var collection = _database.GetCollection<WeatherForecast>("WeatherForecasts");
        await collection.InsertOneAsync(weatherData);
        //return weatherData;
    }

    public async Task DeleteWeatherData(string id)
    {
        var collection = _database.GetCollection<WeatherForecast>("WeatherForecasts");
        await collection.DeleteOneAsync(w => w.Id == id);
    }
    

    public async Task<WeatherForecast> GetTodayWeatherDataByCity(string cityName)
{
    var collection = _database.GetCollection<WeatherForecast>("WeatherForecasts");
    
    var today = DateTime.Today;
    
    return await collection.Find(w => w.CityName == cityName && w.Date >= today && w.Date < today.AddDays(1))
                           .SortByDescending(w => w.Date)
                           .FirstOrDefaultAsync();
}
    public async Task<List<WeatherForecast>> GetWeeklyWeatherForecastByCity(string city)
    {
        var collection = _database.GetCollection<WeatherForecast>("WeatherForecasts");
        var today = DateTime.UtcNow.Date;
        var sevenDaysLater = today.AddDays(7);

        return await collection.Find(w => w.CityName == city && w.Date >= today && w.Date < sevenDaysLater)
                               .SortBy(w => w.Date)
                               .ToListAsync();
    }



    }
}
