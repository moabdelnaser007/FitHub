namespace FitHubBackendAPI.DTOs.Wallet
{
    public class FithubPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CreditsValue { get; set; }
    }
}
