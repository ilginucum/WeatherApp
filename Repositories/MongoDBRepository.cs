using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather_App.Models;
using Weather_App.Options;

namespace Weather_App.Repositories
{
    public class MongoDbRepository
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
    }
}