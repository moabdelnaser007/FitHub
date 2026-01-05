using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.UserDTOs
{
    public class GymSearchQueryDto
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }

        public int? MinRating { get; set; }
        public int? MaxVisitCredits { get; set; }

        // Flags enum
        public GymAmenity? Amenities { get; set; }
    }
}
