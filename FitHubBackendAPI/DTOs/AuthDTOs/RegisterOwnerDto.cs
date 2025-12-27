using FitHubBackendAPI.Validators;
using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class RegisterOwnerDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(50, ErrorMessage = "Full name cannot exceed 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(010|011|012|015)[0-9]{8}$",ErrorMessage = "Please enter a valid Egyptian phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City name cannot exceed 50 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Commercial registration number is required.")]
        [StringLength(100, ErrorMessage = "Commercial registration number cannot exceed 100 characters.")]
        public string CommercialRegistrationNumber { get; set; } = string.Empty;

        //[Required(ErrorMessage = "License file is required.")]
        //[FileValidation(
        //    AllowedExtensions = new[] { ".pdf" },
        //    AllowedContentTypes = new[] { "application/pdf" },
        //    MaxFileSizeInBytes = 5 * 1024 * 1024, // 5 MB limit (tweak if needed)
        //    ErrorMessage = "Only PDF files up to 5 MB are allowed.")]
        //public IFormFile LicenseFile { get; set; } = default!;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase, and number" )]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
                ErrorMessage = "Password must contain uppercase, lowercase, and number")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
