using Twikey.Modal;
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
    public class PaylinkGateway
    {
        private readonly TwikeyClient _twikeyClient;

        protected internal PaylinkGateway(TwikeyClient twikeyClient)
        {
            _twikeyClient = twikeyClient;
        }

        /*
         * <ul>
         * <li>title	Message to the debtor [*1]	Yes	string (200)</li>
         * <li>remittance	Payment message, if empty then title will be used [*2]	No	string</li>
         * <li>amount	Amount to be billed	Yes	string</li>
         * <li>redirectUrl	Optional redirect after pay url (must use http(s)://)	No	url</li>
         * <li>place	Optional place	No	string</li>
         * <li>expiry	Optional expiration date	No	date</li>
         * <li>sendInvite	Send out invite email or sms directly (email, sms)	No	string</li>
         * <li>method	Circumvents the payment selection with PSP (bancontact/ideal/maestro/mastercard/visa/inghomepay/kbc/belfius)	No	string</li>
         * <li>invoice	create payment link for specific invoice number	No	string</li>
         * </ul>
        */
        /// <param name="ct">Template to use can be found @ https://www.twikey.com/r/admin#/c/template</param>
        /// <param name="customer">Customer details</param>
        /// <param name="mandateDetails">Map containing any of the parameters in the above table</param>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public JObject Create(long ct, Customer customer, Dictionary<string, string> linkDetails)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>(linkDetails);
            parameters.Add("ct", ct.ToString());
            if (customer != null)
            {
                parameters.Add("customerNumber", customer.CustomerNumber);
                parameters.Add("email", customer.Email);
                parameters.Add("firstname", customer.Firstname);
                parameters.Add("lastname", customer.Lastname);
                parameters.Add("l", customer.Lang);
                parameters.Add("address", customer.Street);
                parameters.Add("city", customer.City);
                parameters.Add("zip", customer.Zip);
                parameters.Add("country", customer.Country);
                parameters.Add("mobile", customer.Mobile);

                if (customer.CompanyName != null)
                {
                    parameters.Add("companyName", customer.CompanyName);
                    parameters.Add("coc", customer.Coc);
                }
            }

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl("/payment/link");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = _twikeyClient.Send(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (Stream contentStream = response.Content.ReadAsStreamAsync().Result)
                {
                    /* 
                       {
                        "mndtId": "COREREC01",
                        "url": "http://twikey.to/myComp/ToYG",
                        "key": "ToYG"
                       } 
                    */
                    return JObject.Load(new JsonTextReader(new StreamReader(contentStream)));
                }
            }

            String apiError = response.Headers.GetValues("ApiError").First<string>();
            throw new TwikeyClient.UserException(apiError);

        }


        /// Get updates about all links
        /// <param name="paylinkCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public void Feed(IPaylinkCallback paylinkCallback)
        {
            Uri myUrl = _twikeyClient.GetUrl("/payment/link/feed");
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
                        JArray messagesArr = JArray.FromObject(json["Messages"]);
                        isEmpty = messagesArr.Count == 0;
                        if (!isEmpty)
                        {
                            for (int i = 0; i < messagesArr.Count; i++)
                            {
                                JObject obj = (JObject)messagesArr[i];
                                paylinkCallback.Paylink(obj);
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