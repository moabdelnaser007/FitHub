using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "Branch is required")]
        public int BranchId { get; set; } // رايح أنهي فرع

        public int? SubscriptionId { get; set; } // لو عنده اشتراك يبعته، لو زيارة ع الطاير يسيبه فاضي

        [Required(ErrorMessage = "Date and Time is required")]
        public DateTime ScheduledDateTime { get; set; } // ميعاد الحجز
    }
}
