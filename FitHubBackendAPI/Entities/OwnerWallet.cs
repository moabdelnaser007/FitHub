namespace FitHubBackendAPI.Entities
{
    public class OwnerWallet : BaseEntity
    {
        public int WalletId { get; set; }
        public int OwnerId { get; set; }

        public decimal Balance { get; set; }
        public DateTime LastUpdated { get; set; }

        public GymOwner Owner { get; set; } = null!;
    }
}
