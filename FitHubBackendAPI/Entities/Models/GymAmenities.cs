using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.Entities.Models
{
    public class GymAmenities : BaseEntity
    {
        public int BranchId { get; set; }

        public GymAmenity Amenity { get; set; }
        public string? Icon { get; set; }

        public GymBranch? Branch { get; set; }
    }
}
