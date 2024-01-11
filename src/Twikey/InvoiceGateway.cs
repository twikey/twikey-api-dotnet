using Twikey.Model;
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
    public class InvoiceGateway : Gateway
    {
        protected internal InvoiceGateway(TwikeyClient twikeyClient): base(twikeyClient){}

        /// <param name="ct">Template to use can be found @ https://www.twikey.com/r/admin#/c/template</param>
        /// <param name="customer">Customer details</param>
        /// <param name="invoiceDetails">Details specific to the invoice</param>
        /// <returns cref="Invoice">Saved invoice</returns>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public Invoice Create(long ct, Customer customer, Invoice invoice)
        {
            if(customer != null)
            {
                invoice.Customer = customer;
            }

            if(ct > 0)
            {
                invoice.Ct = ct;
            }

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl("/invoice");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

            var invoice_string = JsonConvert.SerializeObject(invoice, Formatting.Indented);
            request.Content = new StringContent(invoice_string, Encoding.UTF8, TwikeyClient.JSON);

            HttpResponseMessage response = _twikeyClient.Send(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<Invoice>(responseString);
            }

            String apiError = response.Headers.GetValues("ApiError").First<string>();
            throw new TwikeyClient.UserException(apiError);
        }

        // Get updates about all invoices (new/updated/cancelled)
        /// <param name="invoiceCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public IEnumerable<Invoice> Feed(params string[] sideloads)
        {
            string url = "/invoice";
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
                    var responseString = response.Content.ReadAsStringAsync().Result;
                    var feed = JsonConvert.DeserializeObject<InvoiceUpdates>(responseString);
                    foreach(var invoice in feed.Invoices)
                    {
                        yield return invoice;
                    }
                    isEmpty = !feed.Invoices.Any();
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
