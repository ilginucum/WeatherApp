using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class UserRegistration
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public bool userType { get; set; }
    public string DefaultCity { get; set; }
    public string Status { get; set; }
   
    
  
}

