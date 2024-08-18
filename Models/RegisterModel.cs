using System.ComponentModel.DataAnnotations;

namespace Weather_App.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "City")]
        public string DefaultCity { get; set; }  // This matches the view

        [Required]
        [Display(Name = "User Type")]
        public string UserType { get; set; }

        // The Status property is not included here as it is set automatically in the controller
    }
}
