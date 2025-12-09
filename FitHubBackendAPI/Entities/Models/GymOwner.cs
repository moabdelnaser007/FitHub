using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Entities
{
    public class GymOwner : BaseEntity
    {

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        public string PasswordHash { get; set; } = null!;

        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.PENDING;
        public string? DocumentUrl { get; set; }
        public string? RejectionReason { get; set; }


        public string CommercialRegistrationNumber { get; set; }
        public string LicenseFileUrl { get; set; }

        public AccountStatus Status { get; set; }

        // Navigation
        public OwnerWallet? Wallet { get; set; }
        public List<GymBranch> Branches { get; set; } = new();
        public List<OwnerSettlement> Settlements { get; set; } = new();
    }
}
