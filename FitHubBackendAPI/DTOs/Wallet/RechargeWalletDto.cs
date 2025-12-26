using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.Wallet
{
    public class RechargeWalletDto
    {
        [Required]
        public int PlanId { get; set; }
    }
}
