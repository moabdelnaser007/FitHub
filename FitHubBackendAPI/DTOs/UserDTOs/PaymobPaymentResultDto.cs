namespace FitHubBackendAPI.DTOs.UserDTOs
{
    public class PaymobPaymentResultDto
    {
        
            public bool IsSuccess { get; set; }
            public string PaymentKey { get; set; }
            public string IframeUrl { get; set; }
            public string Error { get; set; }
        
    }
}
