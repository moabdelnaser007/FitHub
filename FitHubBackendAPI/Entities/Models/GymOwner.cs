using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.Entities
{
    public class GymOwner : BaseEntity
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PasswordHash { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.PENDING;

        // Files stored in DB
        public byte[]? Document { get; set; }
        public byte[]? LicenseFile { get; set; }
        public string? LicenseFileType { get; set; }

        public string? RejectionReason { get; set; }

        public string? CommercialRegistrationNumber { get; set; }

        public AccountStatus? Status { get; set; }

        // Navigation
        public OwnerWallet? Wallet { get; set; }
        public List<GymBranch>? Branches { get; set; } = new();
        public List<OwnerSettlement>? Settlements { get; set; } = new();
    }
}
