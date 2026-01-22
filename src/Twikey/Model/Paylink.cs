using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable enable

namespace Twikey.Model
{
    public class Paylinks
    {
        [JsonPropertyName("links")]
        public IEnumerable<Paylink> Links { get; set; } = Array.Empty<Paylink>();
    }

    public class Paylink
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("state")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? State { get; set; }

        public decimal Amount { get; set; }

        [JsonPropertyName("msg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        private string? Message { get; set; }

        [JsonPropertyName("ref")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Reference { get; set; }

        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Url { get; set; }

        [JsonPropertyName("ct")]
        public string? ContractTemplate { get; set; }

        public bool IsPaid()
        {
            return State == "paid";
        }

    }

    public class PaylinkRequest
    {
        public PaylinkRequest(string message,double amount){
            this.Message = message;
            this.Amount = amount;
        }

        /// <summary>
        /// Message to the debtor
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Payment message, if empty then title will be used
        /// </summary>
        public string? Remittance { get; set; }

        /// <summary>
        /// Amount to be billed	Yes	string
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Optional redirect after pay url (must use http(s)://)
        /// </summary>
        public string? RedirectUrl { get; set; }

        /// <summary>
        /// Optional place of purchase
        /// </summary>
        public string? Place { get; set; }

        /// <summary>
        /// Optional expiration date
        /// </summary>
        public DateTime? Expiry { get; set; }

        /// <summary>
        /// Send out invite email or sms directly (email, sms)
        /// </summary>
        public string? SendInvite { get; set; }

        /// <summary>
        /// Circumvents the payment selection with PSP (bancontact/ideal/maestro/mastercard/visa/inghomepay/kbc/belfius)
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// create payment link for specific invoice number
        /// </summary>
        public string? Invoice { get; set; }

        /// <summary>
        /// Template to use can be found @ https://www.twikey.com/r/admin#/c/template
        /// </summary>
        public long Ct { get; set; }

        /// <summary>
        /// Template to use can be found @ https://www.twikey.com/r/admin#/c/template
        /// </summary>
        public string? Tc { get; set; }

        public string? IdempotencyKey { get; set; }
    }
}
