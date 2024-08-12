using Microsoft.AspNetCore.Mvc;
using Weather_App.Models;

public class ProfileController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        // Load current user data to populate the form if necessary
        var model = new ProfileModel
        {
            Username = "CurrentUsername" // Replace with actual username retrieval logic
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Index(ProfileModel model)
    {
        if (ModelState.IsValid)
        {
            // Logic to update the username and password
            // Update the username
            // Update the password if the CurrentPassword matches and NewPassword is provided
            return RedirectToAction("Index", "Profile"); // Redirect to the profile page or a confirmation page
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }
}
