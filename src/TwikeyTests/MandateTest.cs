using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Twikey;
using Twikey.Model;
using System.Threading.Tasks;
using System.Linq;

namespace TwikeyAPITests
{
    [TestClass]
    public class MandateTest
    {
        private static readonly string s_testVersion = "twikey-test/.net-0.1.0";
        private static readonly string _apiKey = Environment.GetEnvironmentVariable("TWIKEY_API_KEY"); // found in https://www.twikey.com/r/admin#/c/settings/api
        private static readonly long _ct = Environment.GetEnvironmentVariable("CT") == null ? 0L : Convert.ToInt64(Environment.GetEnvironmentVariable("CT")); // found @ https://www.twikey.com/r/admin#/c/template
        private static readonly long _ctWik = Environment.GetEnvironmentVariable("CT_WIK") == null ? 0L : Convert.ToInt64(Environment.GetEnvironmentVariable("CT_WIK")); // WIK template ct
        private static readonly long _ctRcc = Environment.GetEnvironmentVariable("CT_RCC") == null ? 0L : Convert.ToInt64(Environment.GetEnvironmentVariable("CT_RCC")); // RCC template ct
        private static readonly string _mandateNumber = Environment.GetEnvironmentVariable("MNDTNUMBER");
        private static Customer _customer;
        private static TwikeyClient _api;

        [ClassInitialize]
        public static void CreateCustomer(TestContext testContext)
        {
            _customer = new Customer()
            {
                CustomerNumber = s_testVersion,
                Email = "no-reply@example.com",
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000",
                Country = "BE",
                Lang = "nl",
                Mobile = "32498665995"
            };
            var url = Environment.GetEnvironmentVariable("TWIKEY_URL");
            _api = new TwikeyClient(_apiKey, true).WithUserAgent("twikey-api-dotnet/msunit");
            if (url != null) _api.WithEndpoint(url);

        }

        [TestMethod]
        public void TestInviteMandateWithoutCustomerDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var signableMandate = _api.Document.Create(_customer, new MandateRequest(_ct));
            // Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
            Assert.IsNotNull(signableMandate);
            Assert.IsFalse(string.IsNullOrEmpty(signableMandate.Url));
        }

        [TestMethod]
        public async Task AsyncTestInviteMandateWithoutCustomerDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var signableMandate = await _api.Document.CreateAsync(_customer, new MandateRequest(_ct));
            // Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
            Assert.IsNotNull(signableMandate);
            Assert.IsFalse(string.IsNullOrEmpty(signableMandate.MandateNumber));
        }

        [TestMethod]
        public void TestInviteMandateWithCustomerDetailsNullAndEmptyFields()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            Customer customer = new Customer()
            {
                CustomerNumber = s_testVersion,
                Email = null,
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000"
            };
            var signableMandate = _api.Document.Create(customer, new MandateRequest(_ct));
            // Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
            Assert.IsNotNull(signableMandate);
            Assert.IsFalse(string.IsNullOrEmpty(signableMandate.Url));
        }

        [TestMethod]
        public async Task AsyncTestInviteMandateWithCustomerDetailsNullAndEmptyFields()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            Customer customer = new Customer()
            {
                CustomerNumber = s_testVersion,
                Email = null,
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000"
            };
            var signableMandate = await _api.Document.CreateAsync(customer, new MandateRequest(_ct));
            // Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
            Assert.IsNotNull(signableMandate);
            Assert.IsFalse(string.IsNullOrEmpty(signableMandate.MandateNumber));
        }

        [TestMethod]
        public void TestInviteMandateCustomerDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var signableMandate = _api.Document.Create(_customer, new MandateRequest(_ct));
            // Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
            Assert.IsNotNull(signableMandate);
            Assert.IsFalse(string.IsNullOrEmpty(signableMandate.Url));
        }

        [TestMethod]
        public async Task AsyncTestInviteMandateCustomerDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var signableMandate = await _api.Document.CreateAsync(_customer, new MandateRequest(_ct));
            // Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
            Assert.IsNotNull(signableMandate);
            Assert.IsFalse(string.IsNullOrEmpty(signableMandate.MandateNumber));
        }

        private static void ImportSignedMandate()
        {
            if (_ct == 0) { Assert.Inconclusive("CT not set"); return; }
            _api.Document.Sign(_customer, new SignRequest(_ct)
            {
                Method = "import",
                MandateNumber = "IMPORT-" + Guid.NewGuid().ToString("N")[..12].ToUpper(),
                Iban = "BE16645348971174",
                SignDate = DateTime.Today,
                Place = "Gent"
            });
        }

        private static void ImportSignedCreditCard()
        {
            if (_ctRcc == 0) { Assert.Inconclusive("CT_RCC not set"); return; }
            _api.Document.Sign(_customer, new SignRequest(_ctRcc)
            {
                Method = "import",
                MandateNumber = "IMPORT-RCC-" + Guid.NewGuid().ToString("N")[..12].ToUpper(),
                Amount = "1",
                SignDate = DateTime.Today,
                Place = "Gent",
                Attributes = new Dictionary<string, string>
                {
                    { "_psptoken", "test-psp-token" },
                    { "_expiry", "2026-12-31" },
                    { "_last", "9999" },
                    { "_cust", "cst_CfC9qgHKwt" }
                }
            });
        }

        private static void ImportSignedWik()
        {
            if (_ctWik == 0) { Assert.Inconclusive("CT_WIK not set"); return; }
            _api.Document.Sign(_customer, new SignRequest(_ctWik) { Method = "import", SignDate = DateTime.Today });
        }

        [TestMethod]
        public void GetMandatesAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            ImportSignedMandate();
            var feed = _api.Document.Feed();
            Assert.IsNotNull(feed);
            foreach(var mandateUpdate in feed)
            {
                Assert.IsNotNull(mandateUpdate);
                if (mandateUpdate.IsCancelled())
                    Assert.IsNotNull(mandateUpdate.OriginalMandateId);
                else
                    Assert.IsNotNull(mandateUpdate.Mandate);
            }
        }

        [TestMethod]
        public async Task AsyncGetMandatesAndDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            ImportSignedMandate();
            var feed = await _api.Document.FeedAsync();
            Assert.IsNotNull(feed);
            foreach (var mandateUpdate in feed)
            {
                Assert.IsNotNull(mandateUpdate);
                if (mandateUpdate.IsCancelled())
                    Assert.IsNotNull(mandateUpdate.OriginalMandateId);
                else
                    Assert.IsNotNull(mandateUpdate.Mandate);
            }
        }

        [TestMethod]
        public void GetMandatesAndDetailsCreditCard(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            ImportSignedCreditCard();
            var feed = _api.Document.Feed("CREDITCARD");
            Assert.IsNotNull(feed);
            foreach(var mandateUpdate in feed)
            {
                Assert.IsNotNull(mandateUpdate);
                if (mandateUpdate.IsCancelled())
                    Assert.IsNotNull(mandateUpdate.OriginalMandateId);
                else
                    Assert.IsNotNull(mandateUpdate.Mandate);
            }
        }

        [TestMethod]
        public async Task AsyncGetMandatesAndDetailsCreditCard()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            ImportSignedCreditCard();
            var feed = await _api.Document.FeedAsync("CREDITCARD");
            Assert.IsNotNull(feed);
            foreach (var mandateUpdate in feed)
            {
                Assert.IsNotNull(mandateUpdate);
                if (mandateUpdate.IsCancelled())
                    Assert.IsNotNull(mandateUpdate.OriginalMandateId);
                else
                    Assert.IsNotNull(mandateUpdate.Mandate);
            }
        }

        [TestMethod]
        public void GetMandatesAndDetailsCreditCardAndWIK(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            ImportSignedCreditCard();
            ImportSignedWik();
            var feed = _api.Document.Feed("CREDITCARD", "WIK");
            Assert.IsNotNull(feed);
            foreach(var mandateUpdate in feed)
            {
                Assert.IsNotNull(mandateUpdate);
                if (mandateUpdate.IsCancelled())
                    Assert.IsNotNull(mandateUpdate.OriginalMandateId);
                else
                    Assert.IsNotNull(mandateUpdate.Mandate);
            }
        }

        [TestMethod]
        public async Task AsyncGetMandatesAndDetailsCreditCardAndWIK()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            ImportSignedCreditCard();
            ImportSignedWik();
            var feed = await _api.Document.FeedAsync("CREDITCARD", "WIK");
            Assert.IsNotNull(feed);
            foreach (var mandateUpdate in feed)
            {
                Assert.IsNotNull(mandateUpdate);
                if (mandateUpdate.IsCancelled())
                    Assert.IsNotNull(mandateUpdate.OriginalMandateId);
                else
                    Assert.IsNotNull(mandateUpdate.Mandate);
            }
        }

        [TestMethod]
        public void CancelMandate()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = _api.Document.Create(_customer, new MandateRequest(_ct));

            _api.Document.CancelMandate(mandate.MandateNumber, "test");
        }

        [TestMethod]
        public void CancelMandateWithNotify()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = _api.Document.Create(_customer, new MandateRequest(_ct));

            _api.Document.CancelMandate(mandate.MandateNumber, "test", true);
        }

        [TestMethod]
        public async Task AsyncCancelMandate()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = await _api.Document.CreateAsync(_customer, new MandateRequest(_ct));

            await _api.Document.CancelMandateAsync(mandate.MandateNumber, "test");
        }

        [TestMethod]
        public async Task AsyncCancelMandateWithNotify()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = await _api.Document.CreateAsync(_customer, new MandateRequest(_ct));

            await _api.Document.CancelMandateAsync(mandate.MandateNumber, "test", true);
        }

        [TestMethod]
        public void GetMandateDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = _api.Document.Create(_customer, new MandateRequest(_ct));

            var details = _api.Document.Details(mandate.MandateNumber, true);
            // Console.WriteLine("Mandate details:" + details);
            Assert.IsNotNull(details);
            Assert.IsNotNull(details.Mandate);
            Assert.IsFalse(string.IsNullOrEmpty(details.Mandate.MndtId));
        }

        [TestMethod]
        public async Task Async_GetMandateDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = await _api.Document.CreateAsync(_customer, new MandateRequest(_ct));

            var details = await _api.Document.DetailsAsync(mandate.MandateNumber, true);
            // Console.WriteLine("Mandate details:" + details);
            Assert.IsNotNull(details);
            Assert.IsNotNull(details.Mandate);
            Assert.IsFalse(string.IsNullOrEmpty(details.Mandate.MndtId));
        }

        [TestMethod]
        public void GetMandatePdf()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = _api.Document.Create(_customer, new MandateRequest(_ct));

            try
            {
                var pdfError = _api.Document.Pdf(mandate.MandateNumber);
                Assert.IsNotNull(pdfError);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Single().Message != "err_no_contract") throw;

                // Console.WriteLine("Mandate has no contract, but endpoint was reached");
                Assert.AreEqual("err_no_contract", ex.InnerExceptions.Single().Message);
            }
        }

        [TestMethod]
        public async Task AsyncGetMandatePdf()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var mandate = await _api.Document.CreateAsync(_customer, new MandateRequest(_ct));

            try
            {
                var pdfError = await _api.Document.PdfAsync(mandate.MandateNumber);
                Assert.IsNotNull(pdfError);
            }
            catch (TwikeyClient.UserException ex)
            {
                if (ex.Message != "err_no_contract") throw;

                // Console.WriteLine("Mandate has no contract, but endpoint was reached");
                Assert.AreEqual("err_no_contract", ex.Message);
            }
        }
    }
}
