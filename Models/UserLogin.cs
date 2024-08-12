using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



public class UserLogin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string LogId { get; set; }
    public char LogTime { get; set; }
    public char Ip { get; set; }
    
}
