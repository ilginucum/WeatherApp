namespace Weather_App.Models
{
    public class OpenWeatherMapResponse
    {
        public string Name { get; set; }
        public OpenWeatherMapMain Main { get; set; }
        public List<OpenWeatherMapWeather> Weather { get; set; }
    }

    public class OpenWeatherMapForecastResponse
    {
        public List<OpenWeatherMapForecastItem> List { get; set; }
    }

    public class OpenWeatherMapForecastItem
    {
        public string Name { get; set; }
        public DateTime DtTxt { get; set; }
        public OpenWeatherMapMain Main { get; set; }
        public List<OpenWeatherMapWeather> Weather { get; set; }
    }

    public class OpenWeatherMapMain
    {
        public double Temp { get; set; }
    }

    public class OpenWeatherMapWeather
    {
        public string Main { get; set; }
    }
}