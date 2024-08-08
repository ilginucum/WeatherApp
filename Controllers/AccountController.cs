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
                // Replace with your real authentication logic
                if (model.Username == "user" && model.Password == "password")
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

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Replace with real user creation logic
                if (model.Username != "existinguser")
                {
                    // In a real application, you'd add the user to the database here
                    // Redirect to login page after successful registration
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", "Username already exists.");
                }
            }
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
