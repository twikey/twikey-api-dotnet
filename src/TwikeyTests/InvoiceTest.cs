using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Twikey;
using Twikey.Model;
using System.Threading.Tasks;

namespace TwikeyAPITests
{
    [TestClass]
    public class InvoiceTest
    {
        private static readonly string s_testVersion = "twikey-test/.net-0.1.0";
        private static readonly string _apiKey = Environment.GetEnvironmentVariable("TWIKEY_API_KEY"); // found in https://www.twikey.com/r/admin#/c/settings/api
        private static readonly long _ct = Environment.GetEnvironmentVariable("CT") == null ? 0L : Convert.ToInt64(Environment.GetEnvironmentVariable("CT")); // found @ https://www.twikey.com/r/admin#/c/template
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
            _api = new TwikeyClient(_apiKey, true).WithUserAgent("twikey-api-dotnet/msunit");

        }

        [TestMethod]
        public void TestCreateInvoiceWithCustomerNullEmtpyFields()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            Customer customer = new Customer()
            {
                Email = "no-reply@example.com",
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000",
                Mobile = null
            };
            var invoice = new Invoice()
            {
                Number = "Invoice-2-"+DateTime.Now.ToString("yyyyMMdd"),
                Title = "Invoice April",
                Remittance = s_testVersion,
                Amount = 10.90,
                Date = DateTime.Now,
                Duedate = DateTime.Now.AddDays(30),
                Customer = customer,
            };

            invoice = _api.Invoice.Create(_ct, customer, invoice);
            // Console.WriteLine("New invoice: " + JsonSerializer.Serialize(invoice, new JsonSerializerOptions{WriteIndented = true}));
            Assert.IsNotNull(invoice);
        }

        [TestMethod]
        public async Task AsyncTestCreateInvoiceWithCustomerNullEmtpyFields()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            Customer customer = new Customer()
            {
                Email = "no-reply@example.com",
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000",
                Mobile = null
            };
            var invoice = new Invoice()
            {
                Number = "Invoice-2-" + DateTime.Now.ToString("yyyyMMdd"),
                Title = "Invoice April",
                Remittance = s_testVersion,
                Amount = 10.90,
                Date = DateTime.Now,
                Duedate = DateTime.Now.AddDays(30),
                Customer = customer,
            };

            invoice = await _api.Invoice.CreateAsync(_ct, customer, invoice);
            // Console.WriteLine("New invoice: " + JsonSerializer.Serialize(invoice, new JsonSerializerOptions{WriteIndented = true}));
            Assert.IsNotNull(invoice);
        }

        [TestMethod]
        public void GetInvoiceAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var feed = _api.Invoice.Feed();
            Assert.IsNotNull(feed);
            foreach(var invoice in feed)
            {
                Assert.IsNotNull(invoice);
                Assert.IsFalse(string.IsNullOrEmpty(invoice.Number));
            }
        }

        [TestMethod]
        public async Task AsyncGetInvoiceAndDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var invoiceFeed = await _api.Invoice.FeedAsync();
            Assert.IsNotNull(invoiceFeed);
            foreach (var invoice in invoiceFeed)
            {
                Assert.IsNotNull(invoice);
                Assert.IsFalse(string.IsNullOrEmpty(invoice.Number));
            }
            var payments = await _api.Invoice.PaymentAsync();
            Assert.IsNotNull(payments);
            foreach (var payment in payments)
            {
                Assert.IsNotNull(payment);
                Assert.IsFalse(string.IsNullOrEmpty(payment.EventId));
            }
        }

        [TestMethod]
        public void GetInvoicePdf()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var invoice = new Invoice()
            {
                Number = "InvoicePdf" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Title = "Invoice with PDF",
                Remittance = s_testVersion,
                Amount = 10.90,
                Date = DateTime.Now,
                Duedate = DateTime.Now.AddDays(30),
                Pdf = File.ReadAllBytes("TestResources/empty.pdf")
            };

            invoice = _api.Invoice.Create(_ct, _customer, invoice);
            Assert.IsNotNull(invoice);
            Assert.IsFalse(string.IsNullOrEmpty(invoice.Id));

            var pdfStream = _api.Invoice.Pdf(invoice.Id);
            Assert.IsNotNull(pdfStream);
            Assert.IsTrue(pdfStream.Length > 0);
        }

        [TestMethod]
        public async Task AsyncGetInvoicePdf()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            var invoice = new Invoice()
            {
                Number = "InvoicePdf" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                Title = "Invoice with PDF",
                Remittance = s_testVersion,
                Amount = 10.90,
                Date = DateTime.Now,
                Duedate = DateTime.Now.AddDays(30),
                Pdf = File.ReadAllBytes("TestResources/empty.pdf")
            };

            invoice = await _api.Invoice.CreateAsync(_ct, _customer, invoice);
            Assert.IsNotNull(invoice);
            Assert.IsFalse(string.IsNullOrEmpty(invoice.Id));

            var pdfStream = await _api.Invoice.PdfAsync(invoice.Id);
            Assert.IsNotNull(pdfStream);
            Assert.IsTrue(pdfStream.Length > 0);
        }
    }
}
