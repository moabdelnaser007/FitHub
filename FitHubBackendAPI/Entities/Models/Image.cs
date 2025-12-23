using System.ComponentModel.DataAnnotations.Schema;

namespace FitHubBackendAPI.Entities.Models
{
    public class Image: BaseEntity
    {
        public string? imageName { get; set; }
        public string? imagePath { get; set; }
        public int GymId {  get; set; }

        // Navigation
        [ForeignKey("GymId")]
        public GymBranch branch { get; set; }
    }
}
