namespace FitHubBackendAPI.DTOs.Bookings
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchAddress { get; set; } = string.Empty; 
        public string BookingCode { get; set; } = string.Empty;
        public DateTime ScheduledDateTime { get; set; }
        public int CreditsCost { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
