using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;



public class UserLogin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string LogId { get; set; }
    public DateTime LogTime { get; set; }
    public string IpAdress { get; set; }
    public string Log {get; set; }
    public bool IsSuccessful { get; set; }
    
    
}
