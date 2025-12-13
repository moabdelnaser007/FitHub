using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class AdminUpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }

        public UserRole? Role { get; set; }
        public AccountStatus? Status { get; set; }
    }
}
