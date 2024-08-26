using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Weather_App.Models;
using Weather_App.Repositories;
using Weather_App.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace WeatherApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IMongoDBRepository _mongoDbRepository;
        private readonly IAuthenticationService _authenticationService;

        public ProfileController(IMongoDBRepository mongoDbRepository, IAuthenticationService authenticationService)
        {
            _mongoDbRepository = mongoDbRepository;
            _authenticationService = authenticationService;
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
            var originalUsername = User.Identity.Name;
            if (originalUsername == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var user = await _mongoDbRepository.GetUserByUsername(originalUsername);
            if (user == null)
            {
                return NotFound();
            }

            bool isUsernameChanged = false;
            bool isPasswordChanged = false;
            bool isDefaultCityChanged  = false;

            // Update username if changed
            if (!string.IsNullOrEmpty(model.Username) && model.Username != user.Username)
            {
                user.Username = model.Username;
                isUsernameChanged = true;
            }

            // Update default city if changed
            if (!string.IsNullOrEmpty(model.DefaultCity) && model.DefaultCity != user.DefaultCity)
            {
                user.DefaultCity = model.DefaultCity;
                isDefaultCityChanged = true;
            }

            // Update password if all password fields are filled
            if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                // Validate current password
                if (!PasswordHelper.VerifyPassword(model.CurrentPassword, user.Salt, user.Password))
                {
                    ModelState.AddModelError("", "Current password is incorrect.");
                    return View(model);
                }

                // Validate new password
                if (model.NewPassword != model.ConfirmNewPassword)
                {
                    ModelState.AddModelError("", "New password and confirmation do not match.");
                    return View(model);
                }

                // Update password
                var newSalt = PasswordHelper.GenerateSalt();
                user.Salt = newSalt;
                user.Password = PasswordHelper.HashPassword(model.NewPassword, newSalt);
                isPasswordChanged = true;
            }
            else if (!string.IsNullOrEmpty(model.CurrentPassword) || !string.IsNullOrEmpty(model.NewPassword) || !string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                // If any password field is filled but not all, show an error
                ModelState.AddModelError("", "All password fields must be filled to change the password.");
                return View(model);
            }

            // Only update if there are changes
            if (isUsernameChanged || isPasswordChanged || isDefaultCityChanged)
            {
                await _mongoDbRepository.UpdateUser(originalUsername, user);

                // Update authentication cookie if username changed
                if (isUsernameChanged)
                {
                    await UpdateAuthenticationCookie(user.Username);
                }

                TempData["SuccessMessage"] = "Profile updated successfully.";
            }
            else
            {
                TempData["InfoMessage"] = "No changes were made to your profile.";
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task UpdateAuthenticationCookie(string newUsername)
        {
            await _authenticationService.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, null);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, newUsername)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await _authenticationService.SignInAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}

