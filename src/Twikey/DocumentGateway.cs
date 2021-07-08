using Twikey.Modal;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Linq;
using System.Json;

namespace Twikey
{
    public class DocumentGateway
    {
        private readonly TwikeyClient _twikeyClient;

        protected internal DocumentGateway(TwikeyClient twikeyClient)
        {
            _twikeyClient = twikeyClient;
        }

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
        /// <exception cref="com.twikey.TwikeyClient.UserException">When Twikey returns a user error (400)</exception>
        /// <returns>Url to redirect the customer to or to send in an email</returns>
        public JsonObject Create(long ct, Customer customer, Dictionary<string, string> mandateDetails)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>(mandateDetails);
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
            request.RequestUri = _twikeyClient.GetUrl("/invite");
            request.Method = HttpMethod.Post;
            request.Headers.Add("User-Agent", _twikeyClient.UserAgent);
            request.Headers.Add("Content-Type", TwikeyClient.FORM_URL);
            request.Headers.Add("Authorization", _twikeyClient.GetSessionToken());

            request.Content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = _twikeyClient.Send(request);

            if(response.StatusCode == System.Net.HttpStatusCode.OK){
                //TODO
                return null;

            }else{
                String apiError = response.Headers.GetValues("ApiError").First<string>();
                throw new TwikeyClient.UserException(apiError);
            }


        }
    }

}