using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Weather_App.Models;
using Weather_App.Options;
using System.Threading.Tasks;

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
        // Ideally, hash the password before saving
        updateDefinition = updateDefinition.Set(u => u.Password, updatedUser.Password);
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
    }
}
