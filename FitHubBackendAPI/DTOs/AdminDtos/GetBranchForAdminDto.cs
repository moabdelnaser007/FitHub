using FitHubBackendAPI.DTOs.GymBranchDTOs;
using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class GetBranchForAdminDto
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }

        public string? BranchName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        // price in credits for a single visit without a plan
        public decimal VisitCreditsCost { get; set; }
        public string? Description { get; set; }

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public GymGenderType? GenderType { get; set; } = GymGenderType.Mixed;
        public BranchStatus? Status { get; set; } = BranchStatus.ACTIVE;
        public Days? WorkingDays
        {
            get; set;
        }
        public GymAmenity? AmenitiesAvailable { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
