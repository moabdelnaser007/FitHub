using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class GetUserDataDto
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? City { get; set; }
        public UserRole? Role { get; set; }
        public AccountStatus? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<GetUserSubscriptionDataDto>? Subscriptions { get; set; }= new List<GetUserSubscriptionDataDto>();
        public List<GetUserReviewDto>? Reviews { get; set; } = new List<GetUserReviewDto>();

    }
}
