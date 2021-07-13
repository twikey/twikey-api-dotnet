using Twikey.Modal;
using Twikey.Callback;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace Twikey
{
    public class InvoiceGateway
    {
        private readonly TwikeyClient _twikeyClient;

        protected internal InvoiceGateway(TwikeyClient twikeyClient)
        {
            _twikeyClient = twikeyClient;
        }

        /// <param name="ct">Template to use can be found @ https://www.twikey.com/r/admin#/c/template</param>
        /// <param name="customer">Customer details</param>
        /// <param name="invoiceDetails">Details specific to the invoice</param>
        /// <returns>jsonobject {
        //                       "id": "fec44175-b4fe-414c-92aa-9d0a7dd0dbf2",
        //                       "number": "Inv20200001",
        //                       "title": "Invoice July",
        //                       "ct": 1988,
        //                       "amount": "100.00",
        //                       "date": "2020-01-31",
        //                       "duedate": "2020-02-28",
        //                       "status": "BOOKED",
        //                       "url": "https://yourpage.beta.twikey.com/invoice.html?fec44175-b4fe-414c-92aa-9d0a7dd0dbf2"
        //                     }
        /// </returns>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public JObject Create(long ct, Customer customer, Dictionary<string, string> invoiceDetails)
        {
            JObject customerAsJson = new JObject(){
                {"customerNumber",customer.CustomerNumber},
                {"email", customer.Email},
                {"firstname", customer.Firstname},
                {"lastname", customer.Lastname},
                {"l", customer.Lang},
                {"address", customer.Street},
                {"city", customer.City},
                {"zip", customer.Zip},
                {"country", customer.Country},
                {"mobile", customer.Mobile}
            };

            if (customer.CompanyName != null)
            {
                customerAsJson.Add("companyName", customer.CompanyName);
                customerAsJson.Add("coc", customer.Coc);
            }

            JObject invoice = new JObject(){
                {"customer", customerAsJson},
                {"date", invoiceDetails["date"] != null ? invoiceDetails["date"]: DateTime.Now.ToString()},
                {"duedate", invoiceDetails["duedate"] != null ? invoiceDetails["date"]: DateTime.Now.AddMonths(1).ToString()},
                {"ct", ct}
            };

            foreach (KeyValuePair<string, string> entry in invoiceDetails)
            {
                if (!invoice.ContainsKey(entry.Key))
                    invoice.Add(entry.Key, entry.Value);
            }

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl("/invoice");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

            request.Content = new StringContent(invoice.ToString(), Encoding.UTF8, TwikeyClient.JSON);
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


        // Get updates about all invoices (new/updated/cancelled)
        /// <param name="invoiceCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public void Feed(InvoiceCallback invoiceCallback)
        {
            Uri myUrl = _twikeyClient.GetUrl("/invoice");
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
                        JArray messagesArr = JArray.FromObject(json["Invoices"]);
                        isEmpty = messagesArr.Count == 0;
                        if (!isEmpty)
                        {
                            for (int i = 0; i < messagesArr.Count; i++)
                            {
                                JObject obj = (JObject)messagesArr[i];
                                invoiceCallback.Invoice(obj);
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