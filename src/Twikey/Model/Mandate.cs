using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Twikey.Model
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

    public class MandateRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandateRequest"/> class.
        /// </summary>
        /// <param name="ct">Template to use can be found @ https://www.twikey.com/r/admin#/c/template</param>
        public MandateRequest(long ct)
        {
            this.Ct = ct;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MandateRequest"/> class.
        /// </summary>
        /// <param name="tc">Template to use can be found @ https://www.twikey.com/r/admin#/c/template</param>
        public MandateRequest(string tc)
        {
            this.Tc = tc;
        }

        /// <summary>
        /// Template to use can be found @ https://www.twikey.com/r/admin#/c/template
        /// </summary>
        public long Ct { get; set; }

        /// <summary>
        /// Template to use can be found @ https://www.twikey.com/r/admin#/c/template
        /// </summary>
        public string Tc { get; set; }

        /// <summary>
        /// International Bank Account Number of the debtor
        /// </summary>
        public string Iban { get; set; }
        /// <summary>
        /// Bank Identifier Code of the IBAN
        /// </summary>
        public string Bic { get; set; }
        /// <summary>
        /// Mandate Identification number (if not generated)
        /// </summary>
        public string MandateNumber { get; set; }
        /// <summary>
        /// The contract number which can override the one defined in the template.
        /// </summary>
        public string ContractNumber { get; set; }
        /// <summary>
        /// Campaign to include this url in
        /// </summary>
        public string Campaign { get; set; }
        /// <summary>
        /// Optional prefix to use in the url (default companyname)
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// If a mandate already exists, don't prepare a new one (based on email, customerNumber or mandatenumber and + template type(=ct))
        /// </summary>
        public bool Check { get; set; }
        /// <summary>
        /// Send a reminder if contract was not signed after number of days
        /// </summary>
        public int ReminderDays { get; set; }
        /// <summary>
        /// Send out invite email directly
        /// </summary>
        public bool SendInvite { get; set; }
        /// <summary>
        /// Add a contract in base64 format
        /// </summary>
        public string Document { get; set; }
        /// <summary>
        /// In euro for a transaction via a first payment or post signature via an SDD transaction
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// (optional) token to be returned in the exit-url (lenght &lt; 100)
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Always start with the registration page, even with all known mandate details
        /// </summary>
        public bool RequireValidation { get; set; }
    }

    public class SignableMandate
    {
        [JsonProperty("mndtId", NullValueHandling = NullValueHandling.Ignore)]
        public string MandateNumber { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string ShortKey { get; set; }
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
        public string Othr { get; set; }
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
