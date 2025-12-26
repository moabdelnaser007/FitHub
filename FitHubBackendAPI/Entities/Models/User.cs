using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class User : BaseEntity
    {

        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? PasswordHash { get; set; }
        public string? City { get; set; }


        public UserRole? Role { get; set; }
        public AccountStatus? Status { get; set; } = AccountStatus.Active;

        public GymOwner? GymOwner { get; set; }
        public GymStaff? GymStaff { get; set; }
        public UserWallet? Wallet { get; set; }
        public List<UserCreditTransactions>? WalletTransactions { get; set; } = new();
        public List<Subscription>? Subscriptions { get; set; } = new();
        public List<Booking>? Bookings { get; set; } = new();
        public List<Visit>? Visits { get; set; } = new();
        public List<Review>? Reviews { get; set; } = new();
        //for Fithub Plans (Many-to-Many)
        public List<FithubUserPlan>? PurchasedPlans { get; set; } = new();
    }
}
