using Twikey.Model;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Twikey
{
    public class PaylinkGateway : Gateway
    {
        protected internal PaylinkGateway(TwikeyClient twikeyClient): base(twikeyClient){}

        /// <param name="customer">Customer details</param>
        /// <param name="request">Request containing the specifics of the link</param>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public Paylink Create(Customer customer, PaylinkRequest linkrequest)
        {
            var parameters = new Dictionary<string, string>();
            AddIfExists(parameters,"ct", linkrequest.Ct);
            AddIfExists(parameters,"tc", linkrequest.Tc);

            AddIfExists(parameters, "title", linkrequest.Message);
            AddIfExists(parameters, "remittance", linkrequest.Remittance);
            AddIfExists(parameters, "amount", linkrequest.Amount);
            AddIfExists(parameters, "redirectUrl", linkrequest.RedirectUrl);
            AddIfExists(parameters, "place", linkrequest.Place);
            AddIfExists(parameters, "expiry", linkrequest.Expiry);
            AddIfExists(parameters, "sendInvite", linkrequest.SendInvite);
            AddIfExists(parameters, "method", linkrequest.Method);
            AddIfExists(parameters, "invoice", linkrequest.Invoice);

            if (customer != null)
            {
                AddIfExists(parameters,"customerNumber", customer.CustomerNumber);
                AddIfExists(parameters,"email", customer.Email);
                AddIfExists(parameters,"firstname", customer.Firstname);
                AddIfExists(parameters,"lastname", customer.Lastname);
                AddIfExists(parameters,"l", customer.Lang);
                AddIfExists(parameters,"address", customer.Street);
                AddIfExists(parameters,"city", customer.City);
                AddIfExists(parameters,"zip", customer.Zip);
                AddIfExists(parameters,"country", customer.Country);
                AddIfExists(parameters,"mobile", customer.Mobile);

                if (customer.CompanyName != null)
                {
                    AddIfExists(parameters,"companyName", customer.CompanyName);
                    AddIfExists(parameters,"coc", customer.Coc);
                }
            }

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl("/payment/link");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());
            if (!String.IsNullOrEmpty(linkrequest.IdempotencyKey)){
                request.Headers.Add("Idempotency-Key", linkrequest.IdempotencyKey);
            }

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = _twikeyClient.Send(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Paylink>(responseString);
            }

            String apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
            throw new TwikeyClient.UserException(apiError);
        }

        /// Get updates about all links
        /// <param name="paylinkCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public IEnumerable<Paylink> Feed(params string[] sideloads)
        {
            string url = "/payment/link/feed";
            if(sideloads != null && sideloads.Length != 0)
            {
                var extra = Array.ConvertAll(sideloads, sideload => "include="+sideload.ToString());
                url += "?" + string.Join("&",extra);
            }

            Uri myUrl = _twikeyClient.GetUrl(url);
            bool isEmpty;
            do
            {
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = myUrl;
                request.Method = HttpMethod.Get;
                request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
                request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

                HttpResponseMessage response = _twikeyClient.Send(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseText = response.Content.ReadAsStringAsync().Result;
                    var feed = JsonConvert.DeserializeObject<Paylinks>(responseText);
                    foreach(var _paylink in feed.Links)
                    {
                        yield return _paylink;
                    }
                    isEmpty = !feed.Links.Any();
                }
                else
                {
                    String apiError = response.Headers.GetValues("ApiError").First<string>();
                    throw new TwikeyClient.UserException(apiError);
                }
            } while (!isEmpty);
        }
    }
}
