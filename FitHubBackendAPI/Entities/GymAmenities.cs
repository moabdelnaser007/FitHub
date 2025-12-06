namespace FitHubBackendAPI.Entities
{
    public class GymAmenities : BaseEntity
    {
        public int AmenityId { get; set; }
        public int BranchId { get; set; }

        public string Title { get; set; } = null!;
        public string? Icon { get; set; }

        public GymBranch Branch { get; set; } = null!;
    }
}
