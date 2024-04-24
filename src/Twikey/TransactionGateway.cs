using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Net.Http.Headers;
using Twikey.Model;
using static Twikey.TwikeyClient;
using System.Threading.Tasks;

namespace Twikey
{
    public class TransactionGateway : Gateway
    {
        protected internal TransactionGateway(TwikeyClient twikeyClient): base(twikeyClient){}

        /// <param name="mandateNumber">required</param>
        /// <param name="transactionDetails">map with keys (message,ref,amount,place)</param>
        /// <returns cref="TransactionEntry">TransactionEntry</returns>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="UserException">When Twikey returns a user error (400)</exception>
        public TransactionEntry Create(string mandateNumber, TransactionRequest transactionDetails)
        {
            return CreateAsync(mandateNumber, transactionDetails).Result;
        }

        /// <param name="mandateNumber">required</param>
        /// <param name="transactionDetails">map with keys (message,ref,amount,place)</param>
        /// <returns cref="TransactionEntry">TransactionEntry</returns>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="UserException">When Twikey returns a user error (400)</exception>
        public async Task<TransactionEntry> CreateAsync(string mandateNumber, TransactionRequest transactionDetails)
        {
            var parameters = new Dictionary<string, string>();
            AddIfExists(parameters, "mndtId", mandateNumber);
            AddIfExists(parameters, "message", transactionDetails.Message);
            AddIfExists(parameters, "amount", transactionDetails.Amount);
            AddIfExists(parameters, "ref", transactionDetails.Reference);
            AddIfExists(parameters, "place", transactionDetails.Place);
            AddIfExists(parameters, "refase2e", transactionDetails.Refase2e);
            AddIfExists(parameters, "date", transactionDetails.Date);
            AddIfExists(parameters, "reqcolldt", transactionDetails.Reqcolldt);

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl("/transaction");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", await _twikeyClient.GetSessionToken());
            if (!string.IsNullOrEmpty(transactionDetails.IdempotencyKey)){
                request.Headers.Add("Idempotency-Key", transactionDetails.IdempotencyKey);
            }

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await _twikeyClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var tx = JsonConvert.DeserializeObject<Transaction>(responseString);
                return tx.Entries.ElementAtOrDefault(0);
            }

            string apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
            throw new UserException(apiError);
        }

        public IEnumerable<TransactionEntry> Feed(params string[] sideloads)
        {
            bool isEmpty;
            do
            {
                var entries = FeedAsync(sideloads).Result;

                foreach (var tx in entries)
                {
                    yield return tx;
                }
                isEmpty = !entries.Any();

            } while (!isEmpty);
        }

        public async Task<IEnumerable<TransactionEntry>> FeedAsync(params string[] sideloads)
        {
            string url = "/transaction";
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
                var feed = JsonConvert.DeserializeObject<Transaction>(responseText);
                return feed.Entries;
            }
            else
            {
                string apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
                throw new UserException(apiError);
            }
        }

        public void RemoveTransaction(string id = null, string reference = null)
        {
            Task.Run(() => RemoveTransactionAsync(id, reference));
        }

        public async Task RemoveTransactionAsync(string id = null, string reference = null)
        {
            if (string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(reference))
                throw new UserException("Either id or reference must be provided");
            var p = new List<string>();
            if (!string.IsNullOrWhiteSpace(id))
                p.Add($"id={id}");
            if (!string.IsNullOrWhiteSpace(reference))
                p.Add($"ref={reference}");

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl($"/transaction?{string.Join("&", p)}");
            request.Method = HttpMethod.Delete;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", await _twikeyClient.GetSessionToken());

            HttpResponseMessage response = await _twikeyClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return;
            }
            else
            {
                var apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
                throw new UserException(apiError);
            }
        }
    }
}
