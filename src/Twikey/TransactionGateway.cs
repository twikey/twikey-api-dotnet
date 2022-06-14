using Twikey.ICallback;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace Twikey
{
    public class TransactionGateway : Gateway
    {
        protected internal TransactionGateway(TwikeyClient twikeyClient): base(twikeyClient){}

        /// <param name="mandateNumber">required</param>
        /// <param name="transactionDetails">map with keys (message,ref,amount,place)</param>
        /// <returns>json object containing 
        ///                     {
        ///                       "contractId": 325638,
        ///                       "mndtId": "MNDT123",
        ///                       "contract": "Algemene voorwaarden",
        ///                       "amount": 10.0,
        ///                       "id": 381563,
        ///                       "msg": "Monthly payment",
        ///                       "place": null,
        ///                       "ref": null,
        ///                       "date": "2017-09-16T14:32:05Z"
        ///                     }
        /// </returns>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public JObject Create(String mandateNumber, Dictionary<string, string> transactionDetails)
        {
            Dictionary<string, string> parameters = CreateParameters(transactionDetails);
            AddIfExists(parameters, "mndtId", mandateNumber);

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl("/transaction");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = _twikeyClient.Send(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (Stream contentStream = response.Content.ReadAsStreamAsync().Result)
                {

                    JObject obj = JObject.Load(new JsonTextReader(new StreamReader(contentStream)));
                    JArray entries = JArray.FromObject(obj["Entries"]);
                    return (JObject)entries[0];
                }
            }

            String apiError = response.Headers.GetValues("ApiError").First<string>();
            throw new TwikeyClient.UserException(apiError);

        }


        /// <param name="transactionCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public void Feed(ITransactionCallback transactionCallback)
        {
            Uri myUrl = _twikeyClient.GetUrl("/transaction");
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
                    using (Stream contentStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        JObject json = JObject.Load(new JsonTextReader(new StreamReader(contentStream)));
                        JArray entries = JArray.FromObject(json["Entries"]);
                        isEmpty = entries.Count == 0;
                        if (!isEmpty)
                        {
                            for (int i = 0; i < entries.Count; i++)
                            {
                                JObject obj = (JObject)entries[i];
                                transactionCallback.Transaction(obj);
                            }
                        }
                    }

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