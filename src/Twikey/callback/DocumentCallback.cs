using Newtonsoft.Json.Linq;

namespace Twikey.Callback{

    public interface DocumentCallback {
        void NewDocument(JObject newDocument);
        void UpdatedDocument(JObject updatedDocument);
        void CancelledDocument(JObject cancelledDocument);
    }
}