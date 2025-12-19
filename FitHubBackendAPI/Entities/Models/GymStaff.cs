using FitHubBackendAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.Entities.Models
{
    public class GymStaff : BaseEntity
    {
        public int UserId { get; set; }
        public int? BranchId { get; set; }

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public string? Role { get; set; }

        public StaffStatus Status { get; set; } = StaffStatus.ACTIVE;

        // Navigation
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("BranchId")]
        public GymBranch Branch { get; set; } = null!;
        public List<Visit> VisitsCheckInHandled { get; set; } = new();
    }
}
