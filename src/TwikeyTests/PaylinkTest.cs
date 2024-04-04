using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Twikey;
using Twikey.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TwikeyAPITests
{
    [TestClass]
    public class PaylinkTest
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
            _api = new TwikeyClient(_apiKey, true).WithUserAgent("twikey-api-dotnet/msunit");
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
            Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
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
            Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
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
            Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
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
            Console.WriteLine(JsonConvert.SerializeObject(link, Formatting.Indented));
        }

        [TestMethod]
        public void GetPaylinkAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            foreach(var link in _api.Paylink.Feed())
            {
                if(link.IsPaid())
                {
                    Console.WriteLine("Paid paylink: " + JsonConvert.SerializeObject(link, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine("Paylink: " + JsonConvert.SerializeObject(link, Formatting.Indented));
                }
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
            foreach (var link in await _api.Paylink.FeedAsync())
            {
                if (link.IsPaid())
                {
                    Console.WriteLine("Paid paylink: " + JsonConvert.SerializeObject(link, Formatting.Indented));
                }
                else
                {
                    Console.WriteLine("Paylink: " + JsonConvert.SerializeObject(link, Formatting.Indented));
                }
            }
        }
    }
}
