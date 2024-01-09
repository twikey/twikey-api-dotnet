using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twikey.Models.Mandates
{
    internal class MandateFeed
    {
        [JsonProperty("GrpHdr")]
        public Groupheader GroupHeader { get; set; }
        public IEnumerable<MandateFeedMessage> Messages { get; set; }
    }

    public class MandateFeedMessage
    {
        [JsonProperty("Mndt")]
        public Mandate Mandate { get; set; }
        [JsonProperty("AmdmntRsn")]
        public MandateFeedReason AmendmentReason { get; set; }
        [JsonProperty("CxlRsn")]
        public MandateFeedReason CancellationReason { get; set; }
        [JsonProperty("OrgnlMndtId")]
        public string OriginalMandateId { get; set; }
        [JsonProperty("CdtrSchmeId")]
        public string CreditorSchemeId { get; set; }
        [JsonProperty("EvtTime")]
        public DateTime? EventTime { get; set; }
    }

    public class Groupheader
    {
        public DateTime CreDtTm { get; set; }
    }

    public class MandateFeedReason
    {
        [JsonProperty("Orgtr")]
        public object Originator { get; set; }
        [JsonProperty("Rsn")]
        public string Reason { get; set; }
    }
}
