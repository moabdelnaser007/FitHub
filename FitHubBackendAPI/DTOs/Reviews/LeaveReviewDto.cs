using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.Reviews
{
    public class LeaveReviewDto
    {
        [Required]
        public int BookingId { get; set; } // عشان نعرف هو بيقيم أنهي زيارة

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; } // عدد النجوم

        public string? Comment { get; set; } // التعليق (اختياري حسب الصورة)

        public bool IsAnonymous { get; set; } // هل يظهر اسمه ولا لأ
    }
}
