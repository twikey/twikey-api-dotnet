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
    public class MandateTest
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
        public void TestInviteMandateWithoutCustomerDetails()
        {
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            var signableMandate = _api.Document.Create(_customer, new MandateRequest(_ct));
            Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
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
            Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
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
            Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
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
            Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
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
            Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
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
            Console.WriteLine("SignableMandate: " + JsonConvert.SerializeObject(signableMandate));
        }

        [TestMethod]
        public void GetMandatesAndDetails(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            foreach(var mandateUpdate in _api.Document.Feed())
            {
                if(mandateUpdate.IsNew())
                {
                    Console.WriteLine("New mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if(mandateUpdate.IsUpdated())
                {
                    Console.WriteLine("Updated mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if(mandateUpdate.IsCancelled())
                {
                    Console.WriteLine("Cancelled mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
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
            foreach (var mandateUpdate in await _api.Document.FeedAsync())
            {
                if (mandateUpdate.IsNew())
                {
                    Console.WriteLine("New mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if (mandateUpdate.IsUpdated())
                {
                    Console.WriteLine("Updated mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if (mandateUpdate.IsCancelled())
                {
                    Console.WriteLine("Cancelled mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
            }
        }

        [TestMethod]
        public void GetMandatesAndDetailsCreditCard(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            foreach(var mandateUpdate in _api.Document.Feed("CREDITCARD"))
            {
                if(mandateUpdate.IsNew())
                {
                    Console.WriteLine("New mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if(mandateUpdate.IsUpdated())
                {
                    Console.WriteLine("Updated mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if(mandateUpdate.IsCancelled())
                {
                    Console.WriteLine("Cancelled mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
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
            foreach (var mandateUpdate in await _api.Document.FeedAsync("CREDITCARD"))
            {
                if (mandateUpdate.IsNew())
                {
                    Console.WriteLine("New mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if (mandateUpdate.IsUpdated())
                {
                    Console.WriteLine("Updated mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if (mandateUpdate.IsCancelled())
                {
                    Console.WriteLine("Cancelled mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
            }
        }

        [TestMethod]
        public void GetMandatesAndDetailsCreditCardAndWIK(){
            if (_apiKey == null)
            {
                Assert.Inconclusive("apiKey is null");
                return;
            }
            foreach(var mandateUpdate in _api.Document.Feed("CREDITCARD", "WIK"))
            {
                if(mandateUpdate.IsNew())
                {
                    Console.WriteLine("New mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if(mandateUpdate.IsUpdated())
                {
                    Console.WriteLine("Updated mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if(mandateUpdate.IsCancelled())
                {
                    Console.WriteLine("Cancelled mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
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
            foreach (var mandateUpdate in await _api.Document.FeedAsync("CREDITCARD", "WIK"))
            {
                if (mandateUpdate.IsNew())
                {
                    Console.WriteLine("New mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if (mandateUpdate.IsUpdated())
                {
                    Console.WriteLine("Updated mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
                else if (mandateUpdate.IsCancelled())
                {
                    Console.WriteLine("Cancelled mandate: " + JsonConvert.SerializeObject(mandateUpdate, Formatting.Indented));
                }
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

            Console.WriteLine("Mandate details:" + details);
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

            Console.WriteLine("Mandate details:" + details);
        }
    }
}
