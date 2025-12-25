
namespace FitHubBackendAPI.DTOs.Reviews
{
    public class GetAllBranchRevewsDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }

        public DateTime? BookingDate { get; set; }

        public int? Rating { get; set; }
        public string? Comment { get; set; }
    }
}
