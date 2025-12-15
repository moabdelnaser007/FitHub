namespace FitHubBackendAPI.DTOs.Bookings
{
    public class BookingHistoryDto
    {
        public int Id { get; set; } // عشان لو عاوز يدوس عليه يشوف التفاصيل
        public string BranchName { get; set; } = string.Empty; // اسم الفرع (زي الصورة)
        public string BookingCode { get; set; } = string.Empty; // الكود #123456
        public DateTime ScheduledDateTime { get; set; } // التاريخ والوقت
        public int CreditsCost { get; set; } // التكلفة
        public string Status { get; set; } = string.Empty; // الحالة (Confirmed, Completed...)

        public bool HasReview { get; set; } // عشان زرار "Leave Review" يظهر لو لسه معملش ريفيو
    }
}
