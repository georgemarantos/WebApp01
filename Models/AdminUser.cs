using System.ComponentModel.DataAnnotations;

namespace CleanDDTest.Models
{
    public partial class AdminUser
    {
        public int Id { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage ="Please enter the user's email address.")]
        public string UserEmail { get; set; } = null!;

        [Required(ErrorMessage ="Please enter the user's first name.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage ="Please enter the users's last name.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage ="Please enter the users's role.")]
        public string UserRole { get; set; } = null!;

        [Required(ErrorMessage ="Please enter the user's status.")]
        public int UserStatus { get; set; }
        
        [Display(Name = "Password")]
//      [Required(ErrorMessage = "Please enter your new password")]
        [StringLength(15, ErrorMessage = "New password must have a min of {2} characters and max of 15 characters", MinimumLength = 8)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#\$%\^\?&\*\|\+]).{8,15})", ErrorMessage = "New password must contain: at least 1 uppercase alphabet, 1 lowercase alphabet, 1 number and 1 special character")]
        public string? UserPword { get; set; } = null!;

        public bool PasswordReset { get; set; }
    }
}
