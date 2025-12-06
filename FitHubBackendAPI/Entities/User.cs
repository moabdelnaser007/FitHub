using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string? City { get; set; }

        public UserStatus Status { get; set; } = UserStatus.ACTIVE;
        public DateTime CreatedAt { get; set; }

        public UserWallet? Wallet { get; set; }
        public List<UserCreditTransactions> WalletTransactions { get; set; } = new();
        public List<Subscription> Subscriptions { get; set; } = new();
        public List<Booking> Bookings { get; set; } = new();
        public List<Visit> Visits { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
