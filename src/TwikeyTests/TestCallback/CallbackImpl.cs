using Twikey.Callback;
using Newtonsoft.Json.Linq;
using System;

namespace TwikeyAPITests.Callback
{
    public class DocumentCallbackImpl : DocumentCallback
    {

        public void NewDocument(JObject newDocument)
        {
            Console.WriteLine("New mandate: " + newDocument);
        }

        public void UpdatedDocument(JObject updatedDocument)
        {
            Console.WriteLine("Updated mandate: " + updatedDocument);
        }

        public void CancelledDocument(JObject cancelledDocument)
        {
            Console.WriteLine("Cancelled mandate: " + cancelledDocument);
        }

    }

    public class InvoiceCallbackImpl : InvoiceCallback
    {

        public void Invoice(JObject updatedInvoice)
        {
            Console.WriteLine("Updated invoice: " + updatedInvoice);
        }

    }

}