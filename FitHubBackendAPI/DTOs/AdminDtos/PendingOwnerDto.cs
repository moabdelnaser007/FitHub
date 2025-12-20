using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class PendingOwnerDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? CommercialRegistrationNumber { get; set; } = null!;
        //public IFormFile LicenseFile { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
    }
}
