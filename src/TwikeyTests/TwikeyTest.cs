using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Twikey;

namespace TwikeyAPITests
{
    [TestClass]
    public class TwikeyAPITest
    {
        [TestMethod]
        public async Task SendAsync_UsesInjectedHttpClient()
        {
            HttpRequestMessage capturedRequest = null;
            using (var httpClient = new HttpClient(new DelegateHttpMessageHandler(request =>
                   {
                       capturedRequest = request;
                       return new HttpResponseMessage(HttpStatusCode.OK);
                   })))
            {
                var client = new TwikeyClient("api-key", httpClient);
                var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/test");

                var response = await client.SendAsync(request);

                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreSame(request, capturedRequest);
            }
        }

        [TestMethod]
        public async Task GetSessionToken_UsesInjectedHttpClientAndCachesToken()
        {
            int callCount = 0;
            HttpRequestMessage capturedRequest = null;
            using (var httpClient = new HttpClient(new DelegateHttpMessageHandler(request =>
                   {
                       callCount++;
                       capturedRequest = request;
                       var response = new HttpResponseMessage(HttpStatusCode.OK);
                       response.Headers.Add("Authorization", "token-123");
                       return response;
                   })))
            {
                var client = new TestableTwikeyClient("api-key", httpClient, true);
                client.WithUserAgent("twikey-api-dotnet/tests");

                var token = await client.GetSessionTokenAsync();
                var tokenAgain = await client.GetSessionTokenAsync();

                Assert.AreEqual("token-123", token);
                Assert.AreEqual("token-123", tokenAgain);
                Assert.AreEqual(1, callCount);
                Assert.IsNotNull(capturedRequest);
                Assert.AreEqual(HttpMethod.Post, capturedRequest.Method);
                Assert.AreEqual(new Uri("https://api.beta.twikey.com/creditor"), capturedRequest.RequestUri);
                Assert.AreEqual("twikey-api-dotnet/tests", capturedRequest.Headers.GetValues("User-Agent").Single());

                var body = await capturedRequest.Content.ReadAsStringAsync();
                Assert.AreEqual("apiToken=api-key", body);
            }
        }

        [TestMethod]
        public void VerifySignatureAndDecryptAccountInfo()
        {
            // exiturl defined in template http://example.com?mandatenumber={{mandateNumber}}&status={{status}}&signature={{s}}&account={{account}}
            // outcome http://example.com?mandatenumber=MYDOC&status=ok&signature=8C56F94905BBC9E091CB6C4CEF4182F7E87BD94312D1DD16A61BF7C27C18F569&account=2D4727E936B5353CA89B908309686D74863521CAB32D76E8C2BDD338D3D44BBA

            // string outcome = "http://example.com?mandatenumber=MYDOC&status=ok&" +
            //        "signature=8C56F94905BBC9E091CB6C4CEF4182F7E87BD94312D1DD16A61BF7C27C18F569&" +
            //        "account=2D4727E936B5353CA89B908309686D74863521CAB32D76E8C2BDD338D3D44BBA";

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

        private sealed class TestableTwikeyClient : TwikeyClient
        {
            public TestableTwikeyClient(string apiKey, HttpClient httpClient, bool test)
                : base(apiKey, httpClient, test) { }

            public Task<string> GetSessionTokenAsync()
            {
                return base.GetSessionToken();
            }
        }

        private sealed class DelegateHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

            public DelegateHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            {
                _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_handler(request));
            }
        }
    }
}
