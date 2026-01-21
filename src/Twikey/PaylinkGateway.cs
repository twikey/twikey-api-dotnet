using Twikey.Model;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

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
            return CreateAsync(customer, linkrequest).Result;
        }

        /// <param name="customer">Customer details</param>
        /// <param name="request">Request containing the specifics of the link</param>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public async Task<Paylink> CreateAsync(Customer customer, PaylinkRequest linkrequest)
        {
            var parameters = new Dictionary<string, string>();
            AddIfExists(parameters,"ct", linkrequest.Ct);
            AddIfExists(parameters,"tc", linkrequest.Tc);

            AddIfExists(parameters, "title", linkrequest.Message);
            AddIfExists(parameters, "remittance", linkrequest.Remittance);
            AddIfExists(parameters, "amount", linkrequest.Amount);
            AddIfExists(parameters, "redirectUrl", linkrequest.RedirectUrl);
            AddIfExists(parameters, "place", linkrequest.Place);
            AddIfExists(parameters, "expiry", linkrequest.Expiry?.ToString("yyyy-MM-dd"));
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
            request.Headers.Add("Authorization", await _twikeyClient.GetSessionToken());
            if (!string.IsNullOrEmpty(linkrequest.IdempotencyKey)){
                request.Headers.Add("Idempotency-Key", linkrequest.IdempotencyKey);
            }

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await _twikeyClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Paylink>(responseString);
            }

            string apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
            throw new TwikeyClient.UserException(apiError);
        }

        /// Get updates about all links
        /// <param name="paylinkCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public IEnumerable<Paylink> Feed(params string[] sideloads)
        {
            bool isEmpty;
            do
            {
                var links = FeedAsync(sideloads).Result;

                foreach(var link in links)
                {
                    yield return link;
                }
                isEmpty = !links.Any();
            } while (!isEmpty);
        }


        /// Get updates about all links
        /// <param name="paylinkCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public async Task<IEnumerable<Paylink>> FeedAsync(params string[] sideloads)
        {
            string url = "/payment/link/feed";
            if(sideloads != null && sideloads.Length != 0)
            {
                var extra = Array.ConvertAll(sideloads, sideload => "include="+sideload.ToString());
                url += "?" + string.Join("&",extra);
            }

            Uri myUrl = _twikeyClient.GetUrl(url);
            
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = myUrl;
            request.Method = HttpMethod.Get;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", await _twikeyClient.GetSessionToken());

            HttpResponseMessage response = await _twikeyClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseText = await response.Content.ReadAsStringAsync();
                var feed = JsonConvert.DeserializeObject<Paylinks>(responseText);
                return feed.Links;
            }
            else
            {
                string apiError = response.Headers.GetValues("ApiError").First();
                throw new TwikeyClient.UserException(apiError);
            }
        }
    }
}
