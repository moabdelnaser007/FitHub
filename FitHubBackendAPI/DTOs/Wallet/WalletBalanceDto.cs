namespace FitHubBackendAPI.DTOs.Wallet
{
    public class WalletBalanceDto
    {
        //class to return the wallet by "credits" balance
        public decimal Balance { get; set; }
        public int UserId { get;  set; }
    }
}

