using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class GetAllUserForAdminDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public AccountStatus Status { get; set; }
    }
}
