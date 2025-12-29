namespace FitHubBackendAPI.DTOs.Wallet
{
    public class GetFitHubPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Basic, Premium, Gold

        public string? Description { get; set; }

        public decimal Price { get; set; }   
        public decimal? PriceAfterTax { get; set; } // السعر بالجنيه (250, 500, 800)

        public decimal CreditsValue { get; set; }
    }
}
