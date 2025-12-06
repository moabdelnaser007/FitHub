using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities
{
    public class GymOwner : BaseEntity
    {
        public int OwnerId { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        public string PasswordHash { get; set; } = null!;

        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.PENDING;
        public string? DocumentUrl { get; set; }
        public string? RejectionReason { get; set; }

        public OwnerStatus Status { get; set; } = OwnerStatus.ACTIVE;
        public DateTime CreatedAt { get; set; }

        // Navigation
        public OwnerWallet? Wallet { get; set; }
        public List<GymBranch> Branches { get; set; } = new();
        public List<OwnerSettlement> Settlements { get; set; } = new();
    }
}
