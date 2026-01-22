using System.Text.Json.Serialization;

namespace Twikey.Model
{
    public class Customer
    {
        [JsonPropertyName("lastName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Lastname { get; set; }

        [JsonPropertyName("firstName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Firstname { get; set; }

        [JsonPropertyName("email")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Email { get; set; }

        [JsonPropertyName("l")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Lang { get; set; }

        [JsonPropertyName("mobile")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Mobile { get; set; }

        [JsonPropertyName("address")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Street { get; set; }

        [JsonPropertyName("city")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string City { get; set; }

        [JsonPropertyName("zip")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Zip { get; set; }

        [JsonPropertyName("country")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Country { get; set; }

        [JsonPropertyName("customerNumber")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CustomerNumber { get; set; }

        [JsonPropertyName("companyName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string CompanyName { get; set; }

        [JsonPropertyName("coc")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Coc { get; set; }

        public Customer() { }
    }
}
