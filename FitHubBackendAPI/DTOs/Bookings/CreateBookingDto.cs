using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required]
        public int BranchId { get; set; }

        // لو حجز باشتراك
        public int? SubscriptionId { get; set; }

        [Required]
        public DateTime ScheduledDateTime { get; set; }
    }
}
