using System.ComponentModel.DataAnnotations;

namespace Weather_App.Models
{
    public class ProfileModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Default City")]
        public string DefaultCity { get; set; }

        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}
