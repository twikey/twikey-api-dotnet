using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Twikey;
using Twikey.Model;
using Newtonsoft.Json;

namespace TwikeyAPITests
{
    [TestClass]
    public class TransactionTest
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

        [TestMethod]
        public void TestCreateTransaction()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            if (_mandateNumber == null)
            {
                Assert.Inconclusive("mandateNumber is null");
                return;
            }
            var request = new TransactionRequest("MyMessage",10.55);
            // Optional though recommended
            request.Reference = s_testVersion;
            // Optional
            request.Date = DateTime.Now;
            request.Reqcolldt = DateTime.Now;
            request.Place = "MyTest";
            request.Refase2e = true;

            var transaction = _api.Transaction.Create(_mandateNumber, request);
            Console.WriteLine("New transaction: " + JsonConvert.SerializeObject(transaction, Formatting.Indented));
        }

        [TestMethod]
        public void GetTransactionAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            foreach(var transaction in _api.Transaction.Feed())
            {
                Console.WriteLine("Transaction: " + JsonConvert.SerializeObject(transaction, Formatting.Indented));
            }
        }
    }
}
