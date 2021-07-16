<p align="center">
  <img src="https://cdn.twikey.com/img/logo.png" height="64"/>
</p>
<h1 align="center">Twikey API client for .NET</h1>

Want to allow your customers to pay in the most convenient way, then Twikey is right what you need.

Recurring or occasional payments via (Recurring) Credit Card, SEPA Direct Debit or any other payment method by bringing 
your own payment service provider or by leveraging your bank contract.

Twikey offers a simple and safe multichannel solution to negotiate and collect recurring (or even occasional) payments.
Twikey has integrations with a lot of accounting and CRM packages. It is the first and only provider to operate on a
European level for Direct Debit and can work directly with all major Belgian and Dutch Banks. However you can use the
payment options of your favorite PSP to allow other customers to pay as well.

## Requirements ##

To use the Twikey API client, the following things are required:

+ Get yourself a [Twikey account](https://www.twikey.com).
+ .NET core >= 3.1.0
+ Up-to-date OpenSSL (or other SSL/TLS toolkit)

## Installation ##

TODO
https://docs.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices

## How to create anything ##

The api works the same way regardless if you want to create a mandate, a transaction, an invoice or even a paylink.
the following steps should be implemented:

1. Use the Twikey API client to create or import your item.

2. Once available, our platform will send an asynchronous request to the configured webhook
   to allow the details to be retrieved. As there may be multiple items ready for you a "feed" endpoint is provided
   which acts like a queue that can be read until empty till the next time.

3. The customer returns, and should be satisfied to see that the action he took is completed.

Find our full documentation online on [api.twikey.com](https://api.twikey.com).

## Getting started ##

Initializing the Twikey API client using the Requests library. 
and configure your API key which you can find in the [Twikey merchant interface](https://www.twikey.com).

```csharp
TwikeyClient twikeyClient = new TwikeyClient(apiKey).WithUserAgent("myApp");
``` 

## Documents

Invite a customer to sign a SEPA mandate using a specific behaviour template (ct) that allows you to configure 
the behaviour or flow that the customer will experience. This 'ct' can be found in the template section of the settings.

```csharp
Customer customer = new Customer()
            {
                CustomerNumber = "customerNum123",
                Email = "no-reply@example.com",
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000",
                Country = "BE",
                Lang = "nl",
                Mobile = "32412345678"
            };
Dictionary<string,string> extraParams = ...;
long ct = ...; 

JObject invite = twikeyClient.Document.Create(ct, customer, extraParams); 
```

_After creation, the link available in invite['url'] can be used to redirect the customer into the signing flow or even 
send him a link through any other mechanism. Ideally you store the mandatenumber for future usage (eg. sending transactions)._


### Feed


```csharp
//Implement this interface to work with response from Twikey
//JObject -> Newtonsoft.Json.Linq
public interface DocumentCallback 
{
    void NewDocument(JObject newDocument);
    void UpdatedDocument(JObject updatedDocument);
    void CancelledDocument(JObject cancelledDocument);
}

twikeyClient.Document.Feed(new DocumentCallbackImpl());
```

## Invoices
Create new invoices 

```csharp
Customer customer = new Customer()
            {
                CustomerNumber = "customerNum123",
                Email = "no-reply@example.com",
                Firstname = "Twikey",
                Lastname = "Support",
                Street = "Derbystraat 43",
                City = "Gent",
                Zip = "9000",
                Country = "BE",
                Lang = "nl",
                Mobile = "32412345678"
            };
Dictionary<string, string> invoiceDetails = new Dictionary<string,string>();
invoiceDetails.Add("number", "Invss123");
invoiceDetails.Add("title", "Invoice April");
invoiceDetails.Add("remittance", s_testVersion);
invoiceDetails.Add("amount", "10.90");
invoiceDetails.Add("date", "2020-03-20");
invoiceDetails.Add("duedate", "2020-04-28");

twikeyClient.Invoice.Create(_ct, _customer, invoiceDetails);
```

### Feed

Retrieve the list of updates on invoices that had changes since the last call.

```csharp
//Implement this interface to work with response from Twikey
//JObject -> Newtonsoft.Json.Linq
public interface IInvoiceCallback
{
    void Invoice(JObject updatedInvoice);
}

twikeyClient.Invoice.Feed(new InvoiceCallbackImpl());
```

## Paymentlinks
Create a payment link 

**You need an integration like for example iDeal**

```csharp
Dictionary<string,string> paylinkDetails = new Dictionary<string, string>();
paylinkDetails.Add("message",s_testVersion);
paylinkDetails.Add("amount","1");
long _ct = ...;

twikeyClient.Paylink.Create(_ct, _customepaylinkDetails);

```

### Feed

Get payment link feed since the last retrieval

```csharp
//Implement this interface to work with response from Twikey
//JObject -> Newtonsoft.Json.Linq
public interface IPaylinkCallback
{
    void Paylink(JObject paylink);
}

twikeyClient.Paylink.Feed(new PaylinkCallbackImpl());
```

## Transactions

Send new transactions and act upon feedback from the bank.

```csharp

Dictionary<string,string> transactionDetails = nDictionary<string, string>();
transactionDetails.Add("message",s_testVersion);
transactionDetails.Add("amount","1");
String _mandateNumber = ...;

twikeyClient.Transaction.Create(_mandateNumber, transactionDetails);
```

### Feed

```csharp
//Implement this interface to work with response from Twikey
//JObject -> Newtonsoft.Json.Linq
public interface ITransactionCallback
{
    void Transaction(JObject transaction);
}

twikeyClient.Transaction.Feed(new TransactionCallbackImpl());
```

## Webhook ##

When wants to inform you about new updates about documents or payments a `webhookUrl` specified in your api settings be called.  

```csharp
string incomingSignature = request.Headers.GetValues("X-SIGNATURE").First<string>();
string payload = request.Content.ReadAsStringAsync().Result;

boolean valid = twikeyClient.VerifyWebHookSignature(incomingSignature,payload);
```

## API documentation ##

If you wish to learn more about our API, please visit the [Twikey Api Page](https://api.twikey.com).
API Documentation is available in English.

## Want to help us make our API client even better? ##

Want to help us make our API client even better? We
take [pull requests](https://github.com/twikey/twikey-api-python/pulls). 

## Support ##

Contact: [www.twikey.com](https://www.twikey.com)
