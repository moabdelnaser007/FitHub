using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.Entities.Models
{
    public class OwnerWallet : BaseEntity
    {
        public int? OwnerId { get; set; }
        public int? UserId { get; set; }

        public decimal? Balance { get; set; }
        public DateTime? LastUpdated { get; set; }
        
        public GymOwner? Owner { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
