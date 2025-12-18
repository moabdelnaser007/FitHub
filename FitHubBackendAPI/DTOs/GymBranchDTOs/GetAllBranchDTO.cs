using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.GymBranchDTOs
{
    public class GetAllBranchDTO
    {
        //public int Id { get; set; }
        //public int OwnerId { get; set; }

        public string BranchName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }

        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }

        public GymGenderType? GenderType { get; set; }= GymGenderType.Mixed;
        public BranchStatus? Status { get; set; }= BranchStatus.ACTIVE;
    }
}