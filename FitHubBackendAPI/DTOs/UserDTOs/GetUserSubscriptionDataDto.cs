using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;

namespace FitHubBackendAPI.DTOs.UserDTOs
{
    public class GetUserSubscriptionDataDto
    {
        public int Id { get; set; }
        public int PlanId { get; set; }
        public int BranchId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int VisitsAllowed { get; set; }
        public int VisitsUsed { get; set; }

        public SubscriptionStatus Status { get; set; }
        
    }
}
