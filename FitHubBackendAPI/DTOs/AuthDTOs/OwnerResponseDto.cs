namespace FitHubBackendAPI.DTOs.AuthDTOs
{
    public class OwnerResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CommercialRegistrationNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
