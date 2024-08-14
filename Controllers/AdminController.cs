using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Weather_App.Models;
using Weather_App.Repositories;
using System.Threading.Tasks;

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
        public async Task<IActionResult> AddUser(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddUser(user);
                return RedirectToAction(nameof(ManageUsers));
            }
            return View(user);
        }

        public async Task<IActionResult> ManageWeatherData()
        {
            var weatherData = await _repository.GetAllWeatherData();
            return View(weatherData);
        }

        public IActionResult AddWeatherData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWeatherData(WeatherForecast weatherData)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddWeatherData(weatherData);
                return RedirectToAction(nameof(ManageWeatherData));
            }
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
    }
}