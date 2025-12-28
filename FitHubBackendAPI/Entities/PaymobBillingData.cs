using System.Text.Json.Serialization;

namespace FitHubBackendAPI.Entities
{
    public class PaymobBillingData
    {
        [JsonPropertyName("first_name")]
        public string First_Name { get; set; }

        [JsonPropertyName("last_name")]
        public string Last_Name { get; set; }

        [JsonPropertyName("phone_number")]
        public string Phone_Number { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("building")]
        public string Building { get; set; }

        [JsonPropertyName("floor")]
        public string Floor { get; set; }

        [JsonPropertyName("apartment")]
        public string Apartment { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }
    }

}
