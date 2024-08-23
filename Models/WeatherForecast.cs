using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Weather_App.Models
{
    public class WeatherForecast
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime Date { get; set; }
        public int Temperature { get; set; }
        //public int TemperatureF => 32 + (int)(Temperature / 0.5556);
        public string CityName { get; set; }
        public string MainStatus { get; set; }
        //public string IconCode { get; set; }
        public string ImageUrl { get; set; }



    }
}
