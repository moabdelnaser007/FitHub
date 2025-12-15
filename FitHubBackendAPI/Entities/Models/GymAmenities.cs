namespace FitHubBackendAPI.Entities.Models
{
    public class GymAmenities : BaseEntity
    {
        public int BranchId { get; set; }

        public string? Title { get; set; }
        public string? Icon { get; set; }

        public GymBranch? Branch { get; set; }
    }
}
