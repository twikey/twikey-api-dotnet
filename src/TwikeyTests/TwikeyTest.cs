using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twikey;
using Twikey.Modal;
using TwikeyAPITests.Callback;
using System;
using System.Collections.Generic;

namespace TwikeyAPITests
{
    [TestClass]
    public class TwikeyAPITest
    {
        private string _apiKey = Environment.GetEnvironmentVariable("TWIKEY_API_KEY"); // found in https://www.twikey.com/r/admin#/c/settings/api
        private long _ct = Environment.GetEnvironmentVariable("CT") == null ? 0L : Convert.ToInt64(Environment.GetEnvironmentVariable("CT")); // found @ https://www.twikey.com/r/admin#/c/template
        private Customer _customer;
        private TwikeyClient _api;

        [TestInitialize]
        public void CreateCustomer()
        {
            _customer = new Customer()
            {
                CustomerNumber = "customerNum123",
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
            _api = new TwikeyClient(_apiKey, true).WithUserAgent("twikey-api-dotnet/msunit");

        }

        [TestMethod]
        public void TestInviteMandateWithoutCustomerDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            Console.WriteLine(_api.Document.Create(_ct, null, new Dictionary<string, string>()));
        }


        [TestMethod]
        public void TestInviteMandateCustomerDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            Console.WriteLine(_api.Document.Create(_ct, _customer, new Dictionary<string, string>()));
        }

        [TestMethod]
        public void TestCreateInvoice()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            Dictionary<string, string> invoiceDetails = new Dictionary<string, string>();
            invoiceDetails.Add("number", "Invss123");
            invoiceDetails.Add("title", "Invoice April");
            invoiceDetails.Add("remittance", "123456789123");
            invoiceDetails.Add("amount", "10.90");
            invoiceDetails.Add("date", "2020-03-20");
            invoiceDetails.Add("duedate", "2020-04-28");
            Console.WriteLine(_api.Invoice.Create(_ct, _customer, invoiceDetails));
        }

        [TestMethod]
        public void GetMandatesAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            _api.Document.Feed(new DocumentCallbackImpl());
  
        }

        
        [TestMethod]
        public void GetInvoiceAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            _api.Invoice.Feed(new InvoiceCallbackImpl());
  
        }

        [TestMethod]
        public void VerifySignatureAndDecryptAccountInfo()
        {
            // exiturl defined in template http://example.com?mandatenumber={{mandateNumber}}&status={{status}}&signature={{s}}&account={{account}}
            // outcome http://example.com?mandatenumber=MYDOC&status=ok&signature=8C56F94905BBC9E091CB6C4CEF4182F7E87BD94312D1DD16A61BF7C27C18F569&account=2D4727E936B5353CA89B908309686D74863521CAB32D76E8C2BDD338D3D44BBA

            string outcome = "http://example.com?mandatenumber=MYDOC&status=ok&" +
                    "signature=8C56F94905BBC9E091CB6C4CEF4182F7E87BD94312D1DD16A61BF7C27C18F569&" +
                    "account=2D4727E936B5353CA89B908309686D74863521CAB32D76E8C2BDD338D3D44BBA";

            string websiteKey = "BE04823F732EDB2B7F82252DDAF6DE787D647B43A66AE97B32773F77CCF12765";
            string doc = "MYDOC";
            string status = "ok";

            string signatureInOutcome = "8C56F94905BBC9E091CB6C4CEF4182F7E87BD94312D1DD16A61BF7C27C18F569";
            string encryptedAccountInOutcome = "2D4727E936B5353CA89B908309686D74863521CAB32D76E8C2BDD338D3D44BBA";
            Assert.IsTrue(TwikeyClient.VerifyExiturlSignature(websiteKey, doc, status, null, signatureInOutcome));
            string[] ibanAndBic = TwikeyClient.DecryptAccountInformation(websiteKey, doc, encryptedAccountInOutcome);
            Assert.AreEqual("BE08001166979213", ibanAndBic[0]);
            Assert.AreEqual("GEBABEBB", ibanAndBic[1]);
        }

    }
}
