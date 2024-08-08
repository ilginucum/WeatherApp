using System.ComponentModel.DataAnnotations;
namespace Weather_App.Options
{
    public class MongoDbSettings
    {

        public string ConnectionString { get; set; }
        
        public string DatabaseName { get; set; }
    }
}