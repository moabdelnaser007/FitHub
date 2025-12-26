using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.Entities.Models
{
    public class FithubPlan : BaseEntity
    {
        [Required]
        public string Name { get; set; } = string.Empty; // Basic, Premium, Gold

        public string? Description { get; set; }

        public decimal Price { get; set; }       // السعر بالجنيه (250, 500, 800)

        public decimal CreditsValue { get; set; }    // عدد النقاط المقابل (250, 500, 800)

        // Navigation Property (للعلاقة Many-to-Many)
        public List<FithubUserPlan> UserPlans { get; set; } = new();
    }
}
