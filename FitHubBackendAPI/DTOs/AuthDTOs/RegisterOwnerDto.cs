namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class RegisterOwnerDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string CommercialRegistrationNumber { get; set; }
        public IFormFile LicenseFile { get; set; }
    }
}
