using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;

namespace WeatherApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Here you would add authentication logic
                // For demonstration, we'll just check if the username and password are not empty
                if (model.Username == "user" && model.Password == "password") // Replace with real authentication
                {
                    // Redirect to a secure page or dashboard
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
                // Here you would add logic to create a new user
                // For demonstration, we'll just check if the username doesn't already exist
                // In a real application, you'd check against a database
                if (model.Username != "existinguser") // Replace with real user creation logic
                {
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
    }
}