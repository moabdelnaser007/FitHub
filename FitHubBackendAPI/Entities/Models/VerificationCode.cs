using FitHubBackendAPI.Entities.Enums;
  

namespace FitHubBackendAPI.Entities.Models
{
    public class VerificationCode : BaseEntity
    {
        public string Email { get; set; }
        public string Code { get; set; }

        public DateTime ExpireAt { get; set; }
        public DateTime? UsedAt { get; set; }

        public OtpType Type { get; set; }
    }
}
