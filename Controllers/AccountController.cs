using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Weather_App.Models;
using Weather_App.Repositories;
using System.Threading.Tasks;
using Weather_App.Helpers;
using System.Net;
using System.Net.Sockets;
using Weather_App.Extensions;

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
                ipAddress = ipAddress.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
            }

            // Check if the IP is a loopback address
            if (string.IsNullOrEmpty(ipAddress) || 
                IPAddress.IsLoopback(IPAddress.Parse(ipAddress)) ||
                ipAddress == "::1")
            {
                // In development environment, try to get the local IP
                ipAddress = GetLocalIpAddress();
            }

            return ipAddress ?? "IP Address not found";
        }

        private string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
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
    var user = await _mongoDbRepository.GetUserByUsername(model.Username);
    if (user != null)
    {
        // Check if the account is currently locked out
        if (user.FailedLoginAttempts >= 3 && user.LastFailedLoginAttempt.HasValue)
        {
            var lockoutEndTime = user.LastFailedLoginAttempt.Value.AddMinutes(1);
            if (DateTime.UtcNow < lockoutEndTime)
            {
                var remainingLockoutTime = (int)(lockoutEndTime - DateTime.UtcNow).TotalSeconds;
                return Request.IsAjaxRequest() 
                    ? Json(new { locked = true, remainingTime = remainingLockoutTime }) 
                    : View(model);
            }
        }

        // Verify the password
        if (PasswordHelper.VerifyPassword(model.Password, user.Salt, user.Password))
        {
            // Correct password, reset failed attempts and log in
            await _mongoDbRepository.ResetFailedLoginAttempts(model.Username);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, user.UserType)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            var ipAddress = GetUserIpAddress();
            var userLogin = new UserLogin
            {
                Username = model.Username,
                LogId = Guid.NewGuid().ToString(),
                LogTime = DateTime.UtcNow,
                IpAdress = ipAddress,
                IsSuccessful = true
            };
            await _mongoDbRepository.SaveUserLogin(userLogin);

            return Request.IsAjaxRequest() 
                ? Json(new { success = true, redirectUrl = Url.Action("Index", "Home") }) 
                : RedirectToAction("Index", "Home");
        }
        else
        {
            // Incorrect password, increment failed attempts
            await _mongoDbRepository.IncrementFailedLoginAttempts(model.Username);
            user = await _mongoDbRepository.GetUserByUsername(model.Username); // Refresh user data

            var ipAddress = GetUserIpAddress();
            var userLogin = new UserLogin
            {
                Username = model.Username,
                LogId = Guid.NewGuid().ToString(),
                LogTime = DateTime.UtcNow,
                IpAdress = ipAddress,
                IsSuccessful = false
            };
            await _mongoDbRepository.SaveUserLogin(userLogin);

            string errorMessage;
            if (user.FailedLoginAttempts == 1)
            {
                errorMessage = "Invalid login attempt. 2 more tries left.";
            }
            else if (user.FailedLoginAttempts == 2)
            {
                errorMessage = "Invalid login attempt. 1 more try left.";
            }
            else if (user.FailedLoginAttempts >= 3)
            {
                // Account is now locked
                return Request.IsAjaxRequest() 
                    ? Json(new { locked = true, remainingTime = 60 }) 
                    : View(model);
            }
            else
            {
                errorMessage = "Invalid login attempt.";
            }

            ModelState.AddModelError("", errorMessage);
            return Request.IsAjaxRequest() 
                ? Json(new { success = false, errorMessage = errorMessage }) 
                : View(model);
        }
    }
    else
    {
        ModelState.AddModelError("", "Invalid login attempt.");
    }

    return Request.IsAjaxRequest() 
        ? Json(new { success = false, errorMessage = "Invalid login attempt." }) 
        : View(model);
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



