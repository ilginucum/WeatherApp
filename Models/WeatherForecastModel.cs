using System;
using System.ComponentModel.DataAnnotations;

namespace Weather_App.Models
{
    public class WeatherForecastModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Temperature is required")]
        [Range(-100, 100, ErrorMessage = "Temperature must be between -100°C and 100°C")]
        [Display(Name = "Temperature (°C)")]
        public int Temperature { get; set; }

        //[Display(Name = "Temperature (°F)")]
        //public int TemperatureF => 32 + (int)(Temperature / 0.5556);

        [Required(ErrorMessage = "City name is required")]
        [StringLength(100, ErrorMessage = "City name cannot be longer than 100 characters")]
        [Display(Name = "City Name")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Main status is required")]
        [StringLength(50, ErrorMessage = "Main status cannot be longer than 50 characters")]
        [Display(Name = "Main Status")]
        public string MainStatus { get; set; }
        
    }
}