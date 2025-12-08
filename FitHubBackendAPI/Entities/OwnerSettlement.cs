using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities
{
    public class OwnerSettlement : BaseEntity
    {
        public int OwnerId { get; set; }

        public string SettlementMonth { get; set; } = null!;

        public int TotalCreditsConsumed { get; set; }
        public decimal TotalExpectedPayout { get; set; }

        public SettlementStatus PayoutStatus { get; set; } = SettlementStatus.PENDING;
        public DateTime? PayoutDate { get; set; }

        public string? AdminNotes { get; set; }

        public GymOwner Owner { get; set; } = null!;
    }
}
