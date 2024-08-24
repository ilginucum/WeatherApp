using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Weather_App.Models;
using Weather_App.Repositories;
using System.Threading.Tasks;
using Weather_App.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System;

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

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> ManageUsers(string usernameFilter, string nameFilter, string defaultCityFilter)
        {
            var allUsers = await _repository.GetAllUsers();

            IEnumerable<UserRegistration> filteredUsers = allUsers;

            bool isFiltered = false;

            if (!string.IsNullOrWhiteSpace(usernameFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.Username.Contains(usernameFilter, StringComparison.OrdinalIgnoreCase));
                isFiltered = true;
            }

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.Name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase));
                isFiltered = true;
            }

            if (!string.IsNullOrWhiteSpace(defaultCityFilter))
            {
                filteredUsers = filteredUsers.Where(u => u.DefaultCity.Contains(defaultCityFilter, StringComparison.OrdinalIgnoreCase));
                isFiltered = true;
            }

            if (isFiltered)
            {
                ViewBag.UsernameFilter = usernameFilter;
                ViewBag.NameFilter = nameFilter;
                ViewBag.DefaultCityFilter = defaultCityFilter;
            }
            else
            {
                ViewBag.UsernameFilter = null;
                ViewBag.NameFilter = null;
                ViewBag.DefaultCityFilter = null;
            }

            return View(filteredUsers.ToList());
        }


        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserRegistration user)
        {
            try
            {
                var existingUser = await _repository.GetUserByUsername(user.Username);
                if (existingUser == null)
                {
                    var salt = PasswordHelper.GenerateSalt();
                    var hashedPassword = PasswordHelper.HashPassword(user.Password, salt);

                    user.Password = hashedPassword;
                    user.Salt = salt;
                    user.UserType = "LastUserType";
                    user.Status = "activated";

                    await _repository.AddUser(user);
                    return RedirectToAction("AddUserSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "Username already exists.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            return View(user);
        }

        public IActionResult AddUserSuccess()
        {
            return View();
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> ManageWeatherData(string selectedCity, DateTime? startDate, DateTime? endDate)
        {
            var allWeatherData = await _repository.GetAllWeatherData();
            var cities = allWeatherData.Select(w => w.CityName).Distinct().OrderBy(c => c).ToList();

            IEnumerable<WeatherForecast> filteredWeatherData = allWeatherData;

            if (!string.IsNullOrEmpty(selectedCity) && !selectedCity.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                filteredWeatherData = filteredWeatherData.Where(w => 
                    string.Equals(w.CityName, selectedCity, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate.HasValue)
            {
                filteredWeatherData = filteredWeatherData.Where(w => w.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                filteredWeatherData = filteredWeatherData.Where(w => w.Date <= endDate.Value.Date.AddDays(1).AddSeconds(-1));
            }

            var orderedWeatherData = filteredWeatherData.OrderByDescending(w => w.Date);

            var viewModel = new ManageWeatherDataViewModel
            {
                WeatherForecasts = orderedWeatherData,
                Cities = new SelectList(cities),
                SelectedCity = selectedCity ?? "All",
                StartDate = startDate,
                EndDate = endDate
            };

            return View(viewModel);
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

        public IActionResult AddWeatherData()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWeatherData(WeatherForecast model)
        {
            try
            {
                var weatherData = new WeatherForecast
                {
                    Date = model.Date,
                    Temperature = model.Temperature,
                    CityName = model.CityName,
                    MainStatus = model.MainStatus
                };
                await _repository.AddWeatherData(weatherData);
                return View("AddWeatherDataSuccess", model);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _repository.GetUserByUsername(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserRegistration model)
        {
            try
            {
                var existingUser = await _repository.GetUserByUsername(model.Username);
                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.Email = model.Email;
                existingUser.Name = model.Name;
                existingUser.UserType = model.UserType;
                existingUser.DefaultCity = model.DefaultCity;
                existingUser.Status = model.Status;

                await _repository.EditUser(existingUser);

                return RedirectToAction(nameof(ManageUsers));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            return View(model);
        }
    }
}