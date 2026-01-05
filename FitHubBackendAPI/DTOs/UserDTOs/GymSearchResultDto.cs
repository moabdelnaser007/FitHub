using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.UserDTOs
{
    public class GymSearchResultDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? City { get; set; }

        public decimal Rating { get; set; }
        public int? VisitCreditsCost { get; set; }

        // Flags enum (زي ما هو في الداتابيز)
        public GymAmenity? Amenities { get; set; }
    }
}
