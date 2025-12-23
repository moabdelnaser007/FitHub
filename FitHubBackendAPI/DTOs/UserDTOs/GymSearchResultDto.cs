namespace FitHubBackendAPI.DTOs.UserDTOs
{
    public class GymSearchResultDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;

        public string Address { get; set; } = null!;

        public decimal Rating { get; set; }

        public List<string> Amenities { get; set; } = new();
    }
}
