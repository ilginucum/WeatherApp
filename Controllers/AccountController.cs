using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Weather_App.Models;
using Weather_App.Repositories;
using System.Threading.Tasks;


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
                if (user != null && user.Password == model.Password) // Note: Use proper password hashing in production
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, user.UserType) // Add role claim
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    // Save login information
                    var userLogin = new UserLogin
                    {
                        Username = model.Username,
                        Password = model.Password,
                        LogId = Guid.NewGuid().ToString(), // Generate a new log ID
                        LogTime = DateTime.UtcNow, // Set the current time
                        IpAdress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown" // Get IP address, fallback to "Unknown" if null
                    };
                    await _mongoDbRepository.SaveUserLogin(userLogin);

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
                var existingUser = await _mongoDbRepository.GetUserByUsername(model.Username);
                if (existingUser == null)
                {
                    var userRegistration = new UserRegistration
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = model.Password, // Use proper password hashing in production
                        ConfirmPassword = model.ConfirmPassword,
                        Name = model.Name,
                        UserType = "LastUserType", // Set default user type
                        DefaultCity = model.DefaultCity,
                        Status = "activated"
                    };

                    await _mongoDbRepository.SaveUserRegistration(userRegistration);

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

