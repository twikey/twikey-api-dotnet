using System;
using System.Collections.Generic;
using System.Text;

namespace Twikey.Models.Mandates
{
    public class Mandate
    {
        public string MndtId { get; set; }
        public string LclInstrm { get; set; }
        public Ocrncs Ocrncs { get; set; }
        public string CdtrSchmeId { get; set; }
        public Cdtr Cdtr { get; set; }
        public Dbtr Dbtr { get; set; }
        public string DbtrAcct { get; set; }
        public Dbtragt DbtrAgt { get; set; }
        public string RfrdDoc { get; set; }
        public Splmtrydata[] SplmtryData { get; set; }
    }
    public class Ocrncs
    {
        public string SeqTp { get; set; }
        public string Frqcy { get; set; }
        public Drtn Drtn { get; set; }
    }

    public class Drtn
    {
        public string FrDt { get; set; }
    }

    public class Cdtr
    {
        public string Nm { get; set; }
        public Pstladr PstlAdr { get; set; }
        public string Id { get; set; }
        public string CtryOfRes { get; set; }
        public Ctctdtls CtctDtls { get; set; }
    }

    public class Pstladr
    {
        public string AdrLine { get; set; }
        public string PstCd { get; set; }
        public string TwnNm { get; set; }
        public string Ctry { get; set; }
    }

    public class Ctctdtls
    {
        public string EmailAdr { get; set; }
    }

    public class Dbtr
    {
        public string Nm { get; set; }
        public Pstladr1 PstlAdr { get; set; }
        public string Id { get; set; }
        public string CtryOfRes { get; set; }
        public Ctctdtls1 CtctDtls { get; set; }
    }

    public class Pstladr1
    {
        public string AdrLine { get; set; }
        public string PstCd { get; set; }
        public string TwnNm { get; set; }
        public string Ctry { get; set; }
    }

    public class Ctctdtls1
    {
        public string EmailAdr { get; set; }
    }

    public class Dbtragt
    {
        public Fininstnid FinInstnId { get; set; }
    }

    public class Fininstnid
    {
        public string BICFI { get; set; }
        public string Nm { get; set; }
    }

    public class Splmtrydata
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
