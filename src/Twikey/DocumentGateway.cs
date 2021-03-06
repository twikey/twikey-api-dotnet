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
    public class DocumentGateway : Gateway
    {
        protected internal DocumentGateway(TwikeyClient twikeyClient): base(twikeyClient){}

        /*
          * <ul>
          * <li>iban	International Bank Account Number of the debtor	No	string</li>
          * <li>bic	Bank Identifier Code of the IBAN	No	string</li>
          * <li>mandateNumber	Mandate Identification number (if not generated)	No	string</li>
          * <li>contractNumber	The contract number which can override the one defined in the template.	No	string</li>
          * <li>campaign	Campaign to include this url in	No	string</li>
          * <li>prefix	Optional prefix to use in the url (default companyname)	No	string</li>
          * <li>check	If a mandate already exists, don't prepare a new one (based on email, customerNumber or mandatenumber and + template type(=ct))	No	boolean</li>
          * <li>reminderDays	Send a reminder if contract was not signed after number of days	No	number</li>
          * <li>sendInvite	Send out invite email directly	No	boolean</li>
          * <li>document	Add a contract in base64 format	No	string</li>
          * <li>amount	In euro for a transaction via a first payment or post signature via an SDD transaction	No	string</li>
          * <li>token	(optional) token to be returned in the exit-url (lenght &lt; 100)	No	string</li>
          * <li>requireValidation	Always start with the registration page, even with all known mandate details	No	boolean</li>
          * </ul>
        */
        /// <param name="ct">Template to use can be found @ https://www.twikey.com/r/admin#/c/template</param>
        /// <param name="customer">Customer details</param>
        /// <param name="mandateDetails">Map containing any of the parameters in the above table</param>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        /// <returns>Url to redirect the customer to or to send in an email</returns>
        public JObject Create(long ct, Customer customer, Dictionary<string, string> mandateDetails)
        {
            Dictionary<string, string> parameters = CreateParameters(mandateDetails);
            parameters.Add("ct", ct.ToString());
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
            request.RequestUri = _twikeyClient.GetUrl("/invite");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = _twikeyClient.Send(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                using (Stream contentStream = response.Content.ReadAsStreamAsync().Result)
                {
                    return JObject.Load(new JsonTextReader(new StreamReader(contentStream)));
                }
            }

            String apiError = response.Headers.GetValues("ApiError").First<string>();
            throw new TwikeyClient.UserException(apiError);

        }


        /// Get updates about all mandates (new/updated/cancelled)
        /// <param name="mandateCallback">Callback for every change</param>
        /// <param name="xTypes">Array of x-types. For example CORE,CREDITCARD</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public void Feed(IDocumentCallback mandateCallback, params string[] xTypes)
        {
            Uri myUrl = _twikeyClient.GetUrl("/mandate");
            bool isEmpty;
            do
            {
                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = myUrl;
                request.Method = HttpMethod.Get;
                request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
                request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());
                if(xTypes != null && xTypes.Length != 0)
                    request.Headers.Add("X-TYPES", String.Join(',', xTypes));
                
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
                                if (obj.ContainsKey("CxlRsn"))
                                {
                                    mandateCallback.CancelledDocument(obj);
                                }
                                else if (obj.ContainsKey("AmdmntRsn"))
                                {
                                    mandateCallback.UpdatedDocument(obj);
                                }
                                else
                                {
                                    mandateCallback.NewDocument(obj);
                                }
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