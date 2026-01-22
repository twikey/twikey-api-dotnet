using System.Text.Json.Serialization;

namespace Twikey.Model.Parameters;

public class Meta
{
    [JsonPropertyName("reminderLevel")]
    public int ReminderLevel { get; set; }
    
    [JsonPropertyName("partial")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Partial { get; set; }
    
    [JsonPropertyName("lastError")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string LastError { get; set; }
    
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Name { get; set; }
    
    [JsonPropertyName("paymentMethod")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string PaymentMethod { get; set; }
}
