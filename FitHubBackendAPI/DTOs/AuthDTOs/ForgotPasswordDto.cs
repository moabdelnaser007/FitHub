using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;
    }
}
