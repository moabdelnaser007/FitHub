namespace FitHubBackendAPI.DTOs.AdminDtos
{
    public class GetUserReviewDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public int? BookingId { get; set; }

        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public bool? IsAnonymous { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
