namespace Twikey.Model
{
    public class MandateDetails
    {
        public string State { get; set; }
        public string Collectable { get; set; }
        public Mandate Mandate { get; set; }
    }

    public class MandateDetailResponse
    {
        public Mandate Mndt { get; set; }
    }
}
