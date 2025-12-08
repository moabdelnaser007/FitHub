namespace FitHubBackendAPI.Entities
{
    public class UserWallet : BaseEntity
    {
        public int UserId { get; set; }

        public int Balance { get; set; }
        public DateTime LastUpdated { get; set; }

        public User User { get; set; } = null!;
    }
}
