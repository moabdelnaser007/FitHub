using FitHubBackendAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class RegisterStaffDTO
    {
        //public int BranchId { get; set; }
        public string FullName { get; set; } = null!;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$", ErrorMessage = "Please enter a valid Egyptian phone number.")]
        public string? Phone { get; set; }
        public string?  City { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, and number")]
        public string Password { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
                ErrorMessage = "Password must contain uppercase, lowercase, and number")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.ACTIVE;
    }
}
