namespace FitHubBackendAPI.DTOs.Bookings
{
    public class BookingHistoryDto
    {
        public int Id { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public DateTime ScheduledDateTime { get; set; }
        public decimal CreditsCost { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool HasReview { get; set; }
    }
}
