using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace Twikey.Model
{
    public class Paylinks
    {
        public IEnumerable<Paylink> Links { get; set;}
    }

    public class Paylink
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Amount { get; set; }

        [JsonProperty("msg", NullValueHandling = NullValueHandling.Ignore)]
        private string Message { get; set; }

        [JsonProperty("ref", NullValueHandling = NullValueHandling.Ignore)]
        public string Reference { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("ct", NullValueHandling = NullValueHandling.Ignore)]
        public string ContractTemplate { get; set; }

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
        public string Message { get; set; }

        /// <summary>
        /// Payment message, if empty then title will be used
        /// </summary>
        public string Remittance { get; set; }

        /// <summary>
        /// Amount to be billed	Yes	string
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Optional redirect after pay url (must use http(s)://)
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Optional place of purchase
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Optional expiration date
        /// </summary>
        public DateTime Expiry { get; set; }

        /// <summary>
        /// Send out invite email or sms directly (email, sms)
        /// </summary>
        public string SendInvite { get; set; }

        /// <summary>
        /// Circumvents the payment selection with PSP (bancontact/ideal/maestro/mastercard/visa/inghomepay/kbc/belfius)
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// create payment link for specific invoice number
        /// </summary>
        public string Invoice { get; set; }

        /// <summary>
        /// Template to use can be found @ https://www.twikey.com/r/admin#/c/template
        /// </summary>
        public long Ct { get; set; }

        /// <summary>
        /// Template to use can be found @ https://www.twikey.com/r/admin#/c/template
        /// </summary>
        public string Tc { get; set; }
    }
}
