using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Weather_App.Models;
using Weather_App.Repositories;
using System.Threading.Tasks;
using Weather_App.Helpers;

namespace Weather_App.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        
        private readonly IMongoDBRepository _repository;

        public AdminController(IMongoDBRepository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _repository.GetAllUsers();
            return View(users);
        }


        public IActionResult AddUser()
        {
            return View();
        }

      


        [HttpPost]
public async Task<IActionResult> AddUser(UserRegistration user)
{
    //if (ModelState.IsValid)
    //{
        try
        {
            // Check if the username already exists
            var existingUser = await _repository.GetUserByUsername(user.Username);
            if (existingUser == null)
            {
                // Generate salt and hash the password
                var salt = PasswordHelper.GenerateSalt();
                var hashedPassword = PasswordHelper.HashPassword(user.Password, salt);

                // Set other properties
                user.Password = hashedPassword;
                user.Salt = salt;
                user.UserType = "LastUserType"; // Set default user type
                user.Status = "activated"; // Set default status

                // Save the user registration
                await _repository.AddUser(user);

                // Redirect to another action or success page
                //return RedirectToAction("AddUserSucess"); // Adjust the action name as needed
                return RedirectToAction("AddUserSucess");
            }
            //else
            //{
            //    ModelState.AddModelError("", "Username already exists.");
            //}
        }
        catch (Exception ex)
        {
            // Handle exception and display error
            ModelState.AddModelError("", $"Error: {ex.Message}");
        }
    //}
    // Return the view with the model to show validation errors or other issues
    return View(user);
}
public IActionResult AddUserSuccess()
{
    return View();
}





        public async Task<IActionResult> ManageWeatherData()
        {
            var weatherData = await _repository.GetAllWeatherData();
            return View(weatherData);
        }

        

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _repository.DeleteUser(id);
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWeatherData(string id)
        {
            await _repository.DeleteWeatherData(id);
            return RedirectToAction(nameof(ManageWeatherData));
        }

        //[HttpGet]
        public IActionResult AddWeatherData()
        {
            return View();
        }



     
        [HttpPost]
        //public async Task<IActionResult> AddWeatherData(WeatherForecast model)
        //{
            // Burada herhangi bir işlem yapmıyoruz, sadece başarılı bir yanıt dönüyoruz
                //return Ok("Weather data received successfully");
            //if (ModelState.IsValid)
            //{
                // Prepare the weather data to be saved
                //var weatherData = new WeatherForecast
                //{
                   // Id = Guid.NewGuid().ToString(), // Generate a new weather data ID
                    //Date = model.Date,
                    //Temperature = model.Temperature,
                    //CityName = model.CityName,
                    //MainStatus = model.MainStatus
                //};
                
                // Save the weather data to the repository
                //await _repository.AddWeatherData(weatherData); 
                
                //return RedirectToAction("Index", "Admin"); // Redirect to the weather data list
                //return RedirectToAction(nameof(ManageWeatherData));
            //}
            //return View(model);
        //}

         public async Task<IActionResult> AddWeatherData(WeatherForecast model)
        {
            try
            {
                var weatherData = new WeatherForecast
                {
                    //Id = Guid.NewGuid().ToString(),
                    Date = model.Date,
                    Temperature = model.Temperature,
                    CityName = model.CityName,
                    MainStatus = model.MainStatus
                };
                await _repository.AddWeatherData(weatherData);
                //return View(model);
                return View("AddWeatherDataSuccess", model); // Redirect to a new view for success
                
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }
    }
}