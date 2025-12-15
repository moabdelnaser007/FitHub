using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.PlanDTOs
{
    public class GetPlanByBranchIdDTO
    {
        public int Id { get; set; }
        public int BranchId { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int? CreditsCost { get; set; }
        public int? VisitsLimit { get; set; }
        public int? DurationDays { get; set; }

        public PlanStatus Status { get; set; }
    }
}
