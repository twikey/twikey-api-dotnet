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

namespace Twikey
{
    public class TransactionGateway : Gateway
    {
        protected internal TransactionGateway(TwikeyClient twikeyClient): base(twikeyClient){}

        /// <param name="mandateNumber">required</param>
        /// <param name="transactionDetails">map with keys (message,ref,amount,place)</param>
        /// <returns cref="TransactionEntry">TransactionEntry</returns>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public Twikey.Model.TransactionEntry Create(String mandateNumber, TransactionRequest transactionDetails)
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
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = _twikeyClient.Send(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                var tx = JsonConvert.DeserializeObject<Transaction>(responseString);
                return tx.Entries.ElementAtOrDefault(0);
            }

            String apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
            throw new TwikeyClient.UserException(apiError);
        }

        public IEnumerable<TransactionEntry> Feed(params string[] sideloads)
        {
            string url = "/transaction";
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
                    var feed = JsonConvert.DeserializeObject<Transaction>(responseText);
                    foreach(var tx in feed.Entries)
                    {
                        yield return tx;
                    }
                    isEmpty = !feed.Entries.Any();
                }
                else
                {
                    string apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
                    throw new TwikeyClient.UserException(apiError);
                }
            } while (!isEmpty);
            yield break;
        }
    }
}
