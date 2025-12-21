using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class GetOwnerForAdminDto
    {
        public int Id { get; set; }
        public GetUserDataDto User { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; } 
        public string? DocumentUrl { get; set; }
        public string? RejectionReason { get; set; }

        public string CommercialRegistrationNumber { get; set; }
    }
}
