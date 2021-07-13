using Newtonsoft.Json.Linq;

namespace Twikey.Callback{

    public interface InvoiceCallback {
        void Invoice(JObject updatedInvoice);
    }
}