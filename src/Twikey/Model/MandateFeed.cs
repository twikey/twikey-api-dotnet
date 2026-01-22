using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Twikey.Model
{
    internal class MandateFeed
    {
        [JsonPropertyName("GrpHdr")]
        public Groupheader GroupHeader { get; set; }
        public IEnumerable<MandateFeedMessage> Messages { get; set; }
    }

    public class MandateFeedMessage
    {
        [JsonPropertyName("Mndt")]
        public Mandate Mandate { get; set; }
        [JsonPropertyName("AmdmntRsn")]
        public MandateFeedReason AmendmentReason { get; set; }
        [JsonPropertyName("CxlRsn")]
        public MandateFeedReason CancellationReason { get; set; }
        [JsonPropertyName("OrgnlMndtId")]
        public string OriginalMandateId { get; set; }
        [JsonPropertyName("CdtrSchmeId")]
        public string CreditorSchemeId { get; set; }
        [JsonPropertyName("EvtTime")]
        public DateTime? EventTime { get; set; }
        [JsonPropertyName("EvtId")]
        public long? EventId { get; set; }

        public bool IsNew()
        {
            return AmendmentReason == null && CancellationReason == null;
        }

        public bool IsUpdated()
        {
            return AmendmentReason != null;
        }

        public bool IsCancelled()
        {
            return CancellationReason != null;
        }
    }

    public class Groupheader
    {
        public DateTime CreDtTm { get; set; }
    }

    public class MandateFeedReason
    {
        [JsonPropertyName("Orgtr")]
        public object Originator { get; set; }
        [JsonPropertyName("Rsn")]
        public string Reason { get; set; }
    }

    // be careful when adding values, these get serialized verbatim to the query string
    public enum MandateFeedIncludes
    {
        mandate,
        person,
        signature,
        plan,
        tracker,
        seq,
        cancelled_mandate
    }
}
