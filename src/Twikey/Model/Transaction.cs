using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Twikey.Model
{
    public class Transaction
    {
        [JsonPropertyName("entries")]
        public IEnumerable<TransactionEntry> Entries { get; set;}
    }

    public class TransactionEntry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("contractId")]
        public int ContractId { get; set; }

        [JsonPropertyName("mndtId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string MandateId { get; set; }

        [JsonPropertyName("contract")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Contract { get; set; }

        public decimal Amount { get; set; }

        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        [JsonPropertyName("msg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        private string MessageResponse { set { Message = value; } }

        [JsonPropertyName("place")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Place { get; set; }

        [JsonPropertyName("ref")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Reference { get; set; }

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        public bool IsFinal { get; set; }

        [JsonPropertyName("state")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string State { get; set; }

        [JsonPropertyName("bkdate")]
        public DateTime? BKDate { get; set; }

        [JsonPropertyName("bkerror")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BKError { get; set; }

        [JsonPropertyName("bkmsg")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BKMessage { get; set; }
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
