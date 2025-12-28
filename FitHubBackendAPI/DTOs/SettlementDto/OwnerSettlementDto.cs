using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.SettlementDto
{
    public class OwnerSettlementDto
    {
        public int Id { get; set; }
        public int TotalCreditsConsumed { get; set; }
        public decimal TotalExpectedPayout { get; set; }
        public SettlementStatus PayoutStatus { get; set; } = SettlementStatus.PENDING;
        public DateTime? PayoutDate { get; set; }

        public string? AdminNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
