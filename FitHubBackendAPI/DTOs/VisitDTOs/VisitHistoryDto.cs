namespace FitHubBackendAPI.DTOs.VisitDTOs
{
    public class VisitHistoryDto
    {
        public int VisitId { get; set; }

        public string BranchName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public decimal CreditsDeducted { get; set; }
    }
}
