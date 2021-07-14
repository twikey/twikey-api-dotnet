using Newtonsoft.Json.Linq;

namespace Twikey.ICallback
{
    public interface IDocumentCallback
    {
        void NewDocument(JObject newDocument);
        void UpdatedDocument(JObject updatedDocument);
        void CancelledDocument(JObject cancelledDocument);
    }

    public interface IInvoiceCallback
    {
        void Invoice(JObject updatedInvoice);
    }

    public interface IPaylinkCallback
    {
        void Paylink(JObject paylink);
    }

    public interface ITransactionCallback
    {
        void Transaction(JObject transaction);
    }

}