using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;
using Weather_App.Repositories;
using System.Threading.Tasks;

namespace WeatherApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly MongoDbRepository _mongoDbRepository;

        public ProfileController(MongoDbRepository mongoDbRepository)
        {
            _mongoDbRepository = mongoDbRepository;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            var username = User.Identity.Name;
            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _mongoDbRepository.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileModel
            {
                Username = user.Username,
                DefaultCity = user.DefaultCity
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.Name;
                if (username == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _mongoDbRepository.GetUserByUsername(username);
                if (user == null)
                {
                    return NotFound();
                }

                // Update user details
                if (!string.IsNullOrEmpty(model.Username) && model.Username != user.Username)
                {
                    user.Username = model.Username;
                }

                if (!string.IsNullOrEmpty(model.DefaultCity))
                {
                    user.DefaultCity = model.DefaultCity;
                }

                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    // Check if the current password is correct
                    if (user.Password != model.CurrentPassword)
                    {
                        ModelState.AddModelError("", "Current password is incorrect.");
                        return View(model);
                    }

                    // Check if the new password and confirmation match
                    if (model.NewPassword != model.ConfirmNewPassword)
                    {
                        ModelState.AddModelError("", "New password and confirmation do not match.");
                        return View(model);
                    }

                    // Update password (ensure you hash the new password before saving)
                    user.Password = model.NewPassword;
                }

                await _mongoDbRepository.UpdateUser(user);

                return RedirectToAction("Index", "Home"); // Redirect to a different page or return to profile page
            }

            return View(model);
        }
    }
}

