using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class User : BaseEntity
    {

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string? City { get; set; }


        public UserRole Role { get; set; }
        public AccountStatus Status { get; set; }

        public UserWallet? Wallet { get; set; }
        public List<UserCreditTransactions> WalletTransactions { get; set; } = new();
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
        public List<Visit> Visits { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
