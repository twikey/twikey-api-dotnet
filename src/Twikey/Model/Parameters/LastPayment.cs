using System;
using System.Text.Json.Serialization;

namespace Twikey.Model.Parameters;

public class LastPayment
{
    [JsonPropertyName("method")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PaymentMethod { get; set; }
    
    [JsonPropertyName("action")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Action { get; set; }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("e2e")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string EndToEndReference { get; set; }
    
    [JsonPropertyName("pmtinf")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PaymentInformation { get; set; }
    
    [JsonPropertyName("iban")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Iban { get; set; }
    
    [JsonPropertyName("bic")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Bic { get; set; }
    
    [JsonPropertyName("rc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ReturnCode { get; set; }
    
    [JsonPropertyName("link")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PaymentLink { get; set; }
    
    [JsonPropertyName("msg")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Message { get; set; }
    
    [JsonPropertyName("mndtId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string MandateId { get; set; }
    
    [JsonPropertyName("date")]
    public DateTime TransactionDate { get; set; }
    
}
