using System.Text.Json.Serialization;

namespace FitHubBackendAPI.Entities
{
    public class PaymobIntentionRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "EGP";
        [JsonPropertyName("payment_methods")]
        public object[] Payment_Methods { get; set; }
        [JsonPropertyName("items")]
        public List<PaymobItem> Items { get; set; }
        [JsonPropertyName("billing_data")]
        public PaymobBillingData Billing_Data { get; set; }
        public PaymobCustomer customer { get; set; }
        public int special_reference { get; set; }
        public int expiration {  get; set; }
        public string? redirection_url {  get; set; }
        public string? notification_url { get; set; }
        public string merchant_order_id  { get; set; }
    }

}
