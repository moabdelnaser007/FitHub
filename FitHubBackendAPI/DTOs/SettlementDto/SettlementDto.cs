using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.SettlementDto
{
    public class SettlementDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public SettlementStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PayoutDate { get; set; }
        public string? AdminNotes { get; set; }
    }
}
