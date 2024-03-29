using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace Twikey.Model
{
    public class Transaction
    {
        public IEnumerable<TransactionEntry> Entries { get; set;}
    }

    public class TransactionEntry
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("contractId", NullValueHandling = NullValueHandling.Ignore)]
        public string ContractId { get; set; }

        [JsonProperty("mndtId", NullValueHandling = NullValueHandling.Ignore)]
        public string MandateId { get; set; }

        [JsonProperty("contract", NullValueHandling = NullValueHandling.Ignore)]
        public string Contract { get; set; }

        [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Amount { get; set; }

        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty("msg", NullValueHandling = NullValueHandling.Ignore)]
        private string MessageResponse { set { Message = value; } }

        [JsonProperty("place", NullValueHandling = NullValueHandling.Ignore)]
        public string Place { get; set; }

        [JsonProperty("ref", NullValueHandling = NullValueHandling.Ignore)]
        public string Reference { get; set; }

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Date { get; set; }
    }

    public class TransactionRequest
    {
        public TransactionRequest(string message,double amount){
            this.Message = message;
            this.Amount = amount;
        }

        /// <summary>
        /// Message to the customer.
        /// </summary>
        public string Message { get; set; }

        public double Amount { get; set; }

        /// <summary>
        /// Optional when the transaction should be collected (default is asap)
        /// </summary>
        public DateTime Reqcolldt { get; set; }

        /// <summary>
        /// Optional reference so when you retrieve the status you can update your records
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Optional date of the transaction, when the purchase was made (default is now)
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Optional place of the transaction, where the purchase was made
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// Optional should we use the inital reference as identifier with the bank
        /// </summary>
        public bool Refase2e { get; set; }

        public string IdempotencyKey { get; set; }
    }
}
