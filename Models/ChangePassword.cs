using System.ComponentModel.DataAnnotations;

namespace CleanDDTest.Models
{
    public class ChangePassword
    {

        [Display(Name ="Password")]
        [Required(ErrorMessage = "Please enter your password")]
        public string? userPword { get; set; }

        [Display(Name ="New Password")]
        [Required(ErrorMessage = "Please enter your new password")]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "New password must have a min of {2} characters and max of 15 characters", MinimumLength = 8)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#\$%\^\?&\*\|\+]).{8,15})", ErrorMessage = "New password must contain: at least 1 uppercase alphabet, 1 lowercase alphabet, 1 number and 1 special character")]
        public string? newUserPword { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("newUserPword", ErrorMessage = "The passwords do not match.")]
        [DataType(DataType.Password)]
        public string? ValidateUserPword { get; set; }

        public AdminUser? AdminUser { get; set; }

    }
}
