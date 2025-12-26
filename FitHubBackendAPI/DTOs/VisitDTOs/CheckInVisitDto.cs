using System.ComponentModel.DataAnnotations;

namespace FitHubBackendAPI.DTOs.VisitDTOs
{
    public class CheckInVisitDto
    {
        [Required]
        public string BookingCode { get; set; } = string.Empty;
    }
}
