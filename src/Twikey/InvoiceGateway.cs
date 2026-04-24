using Twikey.Model;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.IO;
using System.Threading.Tasks;

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
            return CreateAsync(ct, customer, invoice).Result;
        }

        /// <param name="ct">Template to use can be found @ https://www.twikey.com/r/admin#/c/template</param>
        /// <param name="customer">Customer details</param>
        /// <param name="invoiceDetails">Details specific to the invoice</param>
        /// <returns cref="Invoice">Saved invoice</returns>
        /// <exception cref="IOException">When no connection could be made</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        public async Task<Invoice> CreateAsync(long ct, Customer customer, Invoice invoice)
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
            request.Headers.Add("Authorization", await _twikeyClient.GetSessionToken());

            var invoice_string = JsonSerializer.Serialize(invoice, JsonOptions);
            request.Content = new StringContent(invoice_string, Encoding.UTF8, TwikeyClient.JSON);

            HttpResponseMessage response = await _twikeyClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Invoice>(responseString, JsonOptions)!;
            }

            string apiError = response.Headers.GetValues("ApiError").First();
            throw new TwikeyClient.UserException(apiError);
        }

        // Get updates about all invoices (new/updated/cancelled)
        /// <param name="invoiceCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public IEnumerable<Invoice> Feed(params string[] sideloads)
        {
            bool isEmpty;
            do
            {
                var invoices = FeedAsync(sideloads).Result;

                foreach (var invoice in invoices)
                {
                    yield return invoice;
                }
                isEmpty = !invoices.Any();
            } while (!isEmpty);
        }

        // Get updates about all invoices (new/updated/cancelled)
        /// <param name="invoiceCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public async Task<IEnumerable<Invoice>> FeedAsync(params string[] sideloads)
        {
            string url = "/invoice";
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
                var responseString = await response.Content.ReadAsStringAsync();
                var feed = JsonSerializer.Deserialize<InvoiceUpdates>(responseString, JsonOptions);
                return feed?.Invoices ?? Array.Empty<Invoice>();
            }
            else
            {
                string apiError = response.Headers.GetValues("ApiError").First();
                throw new TwikeyClient.UserException(apiError);
            }
        }

        // Get updates about all invoices (new/updated/cancelled)
        /// <param name="invoiceCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public IEnumerable<Event> Payment()
        {
            bool isEmpty;
            do
            {
                var payments = PaymentAsync().Result;
                foreach (var payment in payments)
                {
                    yield return payment;
                }
                isEmpty = !payments.Any();
            } while (!isEmpty);
        }

        // Get updates about all invoices (new/updated/cancelled)
        /// <param name="invoiceCallback">Callback for every change</param>
        /// <exception cref="IOException">When a network issue happened</exception>
        /// <exception cref="Twikey.TwikeyClient.UserException">When there was an issue while retrieving the mandates (eg. invalid apikey)</exception>
        public async Task<IEnumerable<Event>> PaymentAsync()
        {
            string url = "/invoice/payment/feed";
            Uri myUrl = _twikeyClient.GetUrl(url);

            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = myUrl;
            request.Method = HttpMethod.Get;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", await _twikeyClient.GetSessionToken());

            HttpResponseMessage response = await _twikeyClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var feed = JsonSerializer.Deserialize<PaymentUpdates>(responseString, JsonOptions);
                return feed?.Payments ?? Array.Empty<Event>();
            }
            else
            {
                string apiError = response.Headers.GetValues("ApiError").First();
                throw new TwikeyClient.UserException(apiError);
            }
        }

        /// <inheritdoc cref="PdfAsync(string)"/>
        public Stream Pdf(string invoiceId)
        {
            return PdfAsync(invoiceId).Result;
        }

        /// <summary>
        /// Retrieve PDF of an invoice
        /// </summary>
        /// <param name="invoiceId">UUID of the invoice</param>
        /// <returns>Stream of the PDF document</returns>
        /// <exception cref="TwikeyClient.UserException"></exception>
        public async Task<Stream> PdfAsync(string invoiceId)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = _twikeyClient.GetUrl($"/invoice/{invoiceId}/pdf");
            request.Method = HttpMethod.Get;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Authorization", await _twikeyClient.GetSessionToken());

            HttpResponseMessage response = await _twikeyClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else
            {
                var apiError = response.Headers.GetValues("ApiError").FirstOrDefault();
                throw new TwikeyClient.UserException(apiError);
            }
        }
    }
}
