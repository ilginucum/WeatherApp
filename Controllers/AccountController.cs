using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Weather_App.Models;
using Weather_App.Repositories;

namespace WeatherApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly MongoDbRepository _mongoDbRepository;

        public AccountController(MongoDbRepository mongoDbRepository)
        {
            _mongoDbRepository = mongoDbRepository;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _mongoDbRepository.GetUserByUsername(model.Username);
                if (user != null && user.Password == model.Password) // Ensure you hash and compare passwords securely in a real app
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Redirect to the home page after successful login
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username already exists in the database
                var existingUser = await _mongoDbRepository.GetUserByUsername(model.Username);
                if (existingUser == null)
                {
                    // Create a new user registration
                    var userRegistration = new UserRegistration
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password, // Hash the password before saving it
                        ConfirmPassword = model.ConfirmPassword,
                        Name = model.Name,
                        UserType = model.UserType,
                        DefaultCity = model.DefaultCity,
                        Status = "activated" // Set default status
                    };

                    // Save the user registration to the database
                    await _mongoDbRepository.SaveUserRegistration(userRegistration);

                    // Redirect to login page after successful registration
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", "Username already exists.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}

