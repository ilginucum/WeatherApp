using System;
using System.Collections.Generic;

namespace Weather_App.Models
{
    public class WeatherApiResponse
    {
        public Location Location { get; set; }
        public Current Current { get; set; }
        public Forecast Forecast { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
    }

    public class Current
    {
        public double Temp_c { get; set; }
        public double Temp_f { get; set; }
        public Condition Condition { get; set; }
    }

    public class Condition
    {
        public string Text { get; set; }
        public string Icon { get; set; }
    }

    public class Forecast
    {
        public List<ForecastDay> Forecastday { get; set; }
    }

    public class ForecastDay
    {
        public DateTime Date { get; set; }
        public Day Day { get; set; }
    }

    public class Day
    {
        public double Avgtemp_c { get; set; }
        public double Avgtemp_f { get; set; }
        public Condition Condition { get; set; }
    }
}