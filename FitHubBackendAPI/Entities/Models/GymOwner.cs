using FitHubBackendAPI.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.Entities.Models
{
    public class GymOwner : BaseEntity
    {
        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.PENDING;
        public string? DocumentUrl { get; set; }
        public string? RejectionReason { get; set; }

        public string CommercialRegistrationNumber { get; set; }


        // Navigation
        public User User { get; set; }
        public OwnerWallet? Wallet { get; set; }
        public List<GymBranch>? Branches { get; set; } = new();
        public List<GymStaff>? StaffMembers { get; set; } = new();
        public List<OwnerSettlement>? Settlements { get; set; } = new();
    }
}
