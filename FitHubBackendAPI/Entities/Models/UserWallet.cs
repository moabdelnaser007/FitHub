namespace FitHubBackendAPI.Entities.Models
{
    public class UserWallet : BaseEntity
    {
        public int? UserId { get; set; }

        public int? Balance { get; set; }= 0;
        public DateTime? LastUpdated { get; set; }

        public User? User { get; set; }
    }
}
