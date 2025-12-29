using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
        public string Otp { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, and number")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
                ErrorMessage = "Password must contain uppercase, lowercase, and number")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
