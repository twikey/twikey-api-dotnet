using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Twikey;
using Twikey.Model;
using System.Threading.Tasks;

namespace TwikeyAPITests
{
    [TestClass]
    public class PaylinkTest
    {
        private static readonly string s_testVersion = "twikey-test/.net-0.1.0";
        private static readonly string _apiKey = Environment.GetEnvironmentVariable("TWIKEY_API_KEY"); // found in https://www.twikey.com/r/admin#/c/settings/api
        private static readonly long _ct = long.TryParse(Environment.GetEnvironmentVariable("CT"), out var ct) ? ct : 0L; // found @ https://www.twikey.com/r/admin#/c/template
        private static readonly string _mandateNumber = Environment.GetEnvironmentVariable("MNDTNUMBER");
        private static Customer _customer;
        private static TwikeyClient _api;

        [ClassInitialize]
        public static void CreateCustomer(TestContext testContext)
        {
            _customer = new Customer()
            {
                CustomerNumber = "123",
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

        // Needs integration in Twikey for example iDeal
        [TestMethod]
        public void TestCreatePaylink()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var request = new PaylinkRequest("Your payment", 10.55)
            {
                Remittance = s_testVersion,
                Ct = _ct,
            };
            var link = _api.Paylink.Create(_customer, request);
            // Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
            Assert.IsNotNull(link);
            Assert.IsTrue(link.Amount > 0);
            Assert.IsTrue(link.Id > 0);
        }

        // Needs integration in Twikey for example iDeal
        [TestMethod]
        public async Task AsyncTestCreatePaylink()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var request = new PaylinkRequest("Your payment", 10.55)
            {
                Remittance = s_testVersion,
                Ct = _ct,
            };
            var link = await _api.Paylink.CreateAsync(_customer, request);
            // Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
            Assert.IsNotNull(link);
            Assert.IsTrue(link.Amount > 0);
            Assert.IsTrue(link.Id > 0);
        }

        [TestMethod]
        public void TestCreatePaylinkWithCustomerNullEmptyFields()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            Customer customer = new Customer()
            {
                Email = null,
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000"
            };

            var request = new PaylinkRequest("Your payment", 10.55);
            request.Remittance = s_testVersion;

            var link = _api.Paylink.Create(customer, request);
            // Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
            Assert.IsNotNull(link);
            Assert.IsTrue(link.Amount > 0);
            Assert.IsTrue(link.Id > 0);
        }

        [TestMethod]
        public async Task AsyncTestCreatePaylinkWithCustomerNullEmptyFields()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }

            Customer customer = new Customer()
            {
                Email = null,
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000"
            };

            var request = new PaylinkRequest("Your payment", 10.55);
            request.Remittance = s_testVersion;

            var link = await _api.Paylink.CreateAsync(customer, request);
            // Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
            Assert.IsNotNull(link);
            Assert.IsTrue(link.Amount > 0);
            Assert.IsTrue(link.Id > 0);
        }

        [TestMethod]
        public void GetPaylinkAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var links = _api.Paylink.Feed();
            Assert.IsNotNull(links);
            foreach(var link in links)
            {
                Assert.IsNotNull(link);
                Assert.IsTrue(link.Amount >= 0);
            }
        }

        [TestMethod]
        public async Task AsyncGetPaylinkAndDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var links = await _api.Paylink.FeedAsync();
            Assert.IsNotNull(links);
            foreach (var link in links)
            {
                Assert.IsNotNull(link);
                Assert.IsTrue(link.Amount >= 0);
            }
        }
    }
}
