using FitHubBackendAPI.Entities.Enums;

namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class SendOtpDto
    {
        public string Email { get; set; } = null!;
        public OtpType Type { get; set; }
    }
}
