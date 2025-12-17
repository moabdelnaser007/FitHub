using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.StuffDTOs
{
    public class GetStuffDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        public StaffStatus Status { get; set; }
    }
}
