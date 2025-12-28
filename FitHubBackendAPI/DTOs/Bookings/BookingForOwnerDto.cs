using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.Bookings
{
    public class BookingForOwnerDto
    {
        public int BookingId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public BookingStatus Status { get; set; }
        public decimal? CreditsCost { get; set; }
    }
}
