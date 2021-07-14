using Twikey.ICallback;
using Newtonsoft.Json.Linq;
using System;

namespace TwikeyAPITests.TestCallback
{
    public class DocumentCallbackImpl : IDocumentCallback
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

    public class InvoiceCallbackImpl : IInvoiceCallback
    {

        public void Invoice(JObject updatedInvoice)
        {
            Console.WriteLine("Updated invoice: " + updatedInvoice);
        }

    }

    public class PaylinkCallbackImpl : IPaylinkCallback
    {

        public void Paylink(JObject paylink)
        {
            Console.WriteLine("Paylink : " + paylink);
        }

    }
    public class TransactionCallbackImpl : ITransactionCallback
    {

        public void Transaction(JObject transaction)
        {
            Console.WriteLine("Transaction: " + transaction);
        }

    }
}