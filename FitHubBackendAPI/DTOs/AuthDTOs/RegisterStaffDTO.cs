using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class RegisterStaffDTO
    {
        public int BranchId { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.ACTIVE;
    }
}
