using Twikey.Model;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
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
            AddIfExists(parameters, "ct", linkrequest.Ct);
            AddIfExists(parameters, "tc", linkrequest.Tc);

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
                AddIfExists(parameters, "customerNumber", customer.CustomerNumber);
                AddIfExists(parameters, "email", customer.Email);
                AddIfExists(parameters, "firstname", customer.Firstname);
                AddIfExists(parameters, "lastname", customer.Lastname);
                AddIfExists(parameters, "l", customer.Lang);
                AddIfExists(parameters, "address", customer.Street);
                AddIfExists(parameters, "city", customer.City);
                AddIfExists(parameters, "zip", customer.Zip);
                AddIfExists(parameters, "country", customer.Country);
                AddIfExists(parameters, "mobile", customer.Mobile);

                if (customer.CompanyName != null)
                {
                    AddIfExists(parameters, "companyName", customer.CompanyName);
                    AddIfExists(parameters, "coc", customer.Coc);
                    AddIfExists(parameters, "vatno", customer.VatNo);
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
                return JsonSerializer.Deserialize<Paylink>(responseString, JsonOptions)!;
            }

            string apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
            throw new TwikeyClient.UserException(apiError);
        }

        /// Get updates about all links
        /// <param name="all">When true, include all non-paid updates too; by default only paid updates are returned</param>
        /// <param name="sideloads">Optional include values (customer, meta, time, refunds)</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public IEnumerable<Paylink> Feed(bool all = false, params string[] sideloads)
        {
            bool isEmpty;
            do
            {
                var links = FeedAsync(all, sideloads).Result;

                foreach(var link in links)
                {
                    yield return link;
                }
                isEmpty = !links.Any();
            } while (!isEmpty);
        }


        /// Get updates about all links
        /// <param name="all">When true, include all non-paid updates too; by default only paid updates are returned</param>
        /// <param name="sideloads">Optional include values (customer, meta, time, refunds)</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public async Task<IEnumerable<Paylink>> FeedAsync(bool all = false, params string[] sideloads)
        {
            var query = new List<string>();
            if (all)
            {
                query.Add("all=true");
            }
            if (sideloads != null)
            {
                foreach (var sideload in sideloads)
                {
                    query.Add("include=" + sideload);
                }
            }

            string url = "/payment/link/feed";
            if (query.Count != 0)
            {
                url += "?" + string.Join("&", query);
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
                var feed = JsonSerializer.Deserialize<Paylinks>(responseText, JsonOptions);
                return feed?.Links ?? Array.Empty<Paylink>();
            }
            else
            {
                string apiError = response.Headers.GetValues("ApiError").First();
                throw new TwikeyClient.UserException(apiError);
            }
        }
    }
}
