using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Twikey.Model.Parameters;

#nullable enable

namespace Twikey.Model
{
    public class InvoiceUpdates
    {
        public IEnumerable<Invoice> Invoices { get; set; } = Array.Empty<Invoice>();
    }

    public class PaymentUpdates
    {
        public IEnumerable<Event> Payments { get; set; } = Array.Empty<Event>();
    }

    public record Event(
        string EventId,
        EventType EventType,
        DateTimeOffset OccurredAt,
        int Amount,
        string Currency,
        Origin Origin,
        Gateway Gateway,
        Dictionary<string, object?> Details,
        EventError? Error
    );

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EventType
    {
        [EnumMember(Value = "payment")]
        Payment,
        [EnumMember(Value = "payment_failure")]
        PaymentFailure,
        [EnumMember(Value = "refund")]
        Refund
    }

    public enum GatewayType
    {
        Bank,
        Psp
    }

    public record Origin(
        string Object,
        string Id,
        string Number,
        string Ref
    );

    public record Gateway(
        int Id,
        string Name,
        GatewayType Type,
        string? Iban
    );

    public record EventError(
        string Code,
        string Description,
        string Category,
        string ExternalCode,
        string Action,
        int ActionStep
    );

    public class Invoice
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Id { get; set; } = null!;

        [JsonPropertyName("number")]
        public string Number { get; set; } = null!;

        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;

        [JsonPropertyName("remittance")]
        public string Remittance { get; set; } = null!;

        [JsonPropertyName("ct")]
        public long Ct { get; set; }

        [JsonPropertyName("manual")]
        public bool Manual { get; set; }

        [JsonPropertyName("state")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? State { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("date")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonPropertyName("duedate")]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime Duedate { get; set; }

        [JsonPropertyName("ref")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Ref { get; set; }

        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Url { get; set; }

        [JsonPropertyName("customerByDocument")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CustomerByDocument { get; set; }

        [JsonPropertyName("customer")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Customer? Customer { get; set; }

        [JsonPropertyName("pdf")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public byte[]? Pdf { get; set; }

        [JsonIgnore]
        public Meta? Meta { get; set; }

        [JsonIgnore]
        public List<LastPayment>? LastPayment { get; set; }
    }

    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                throw new JsonException("Unable to parse empty date string.");
            }

            return DateTime.ParseExact(value, Format, System.Globalization.CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
