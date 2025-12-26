namespace FitHubBackendAPI.DTOs.Wallet
{
    public class TransactionHistoryDto
    {
        public int Id { get; set; }
        public string Date { get; set; } = string.Empty;       // "Oct 15, 2023"
        public string Description { get; set; } = string.Empty; // "Pro Plan", "Class Booking"

        public decimal AmountPaid { get; set; } // الفلوس اللي دفعها ($49.99)
        public decimal Credits { get; set; }        // النقط اللي زادت أو نقصت (+50 / -5)

        public string Type { get; set; } = string.Empty; // نوع العملية
        public bool IsPositive { get; set; }    // عشان الفرونت يلونها (True=أخضر, False=أحمر)
    }
}