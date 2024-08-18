using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Weather_App.Models{ 
public class UserProfile
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; } 
     public string Salt { get; set; } 
    public string Name { get; set; }
   
    public string DefaultCity { get; set; }
   
   
    
  
}
}