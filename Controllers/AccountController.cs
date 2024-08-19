using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Weather_App.Models;
using Weather_App.Repositories;
using System.Threading.Tasks;
using Weather_App.Helpers;

namespace WeatherApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMongoDBRepository _mongoDbRepository;

    public AccountController(IMongoDBRepository mongoDbRepository)
    {
        _mongoDbRepository = mongoDbRepository;
    }

        
         //taking the ip adress               
        public string GetUserIpAddress()
        {
            string ipAddress = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            else
            {
                // If there are multiple IP addresses, take the first one
                ipAddress = ipAddress.Split(',').First();
            }

            return ipAddress ?? "IP Address not found";
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
            // Retrieve user from the repository
        var user = await _mongoDbRepository.GetUserByUsername(model.Username);

        if (user != null)
        {
            // Verify the entered password
            if (PasswordHelper.VerifyPassword(model.Password, user.Salt, user.Password))
            {
                // Create claims for authentication
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
                    // Generate salt and hash the password
                    var salt = PasswordHelper.GenerateSalt();
                    var hashedPassword = PasswordHelper.HashPassword(model.Password, salt);
                    var userRegistration = new UserRegistration
                    {
                        Username = model.Username,
                        Email = model.Email,
                        Password = hashedPassword, // Use proper password hashing in production
                        //ConfirmPassword = model.ConfirmPassword,
                        Salt = salt,
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

