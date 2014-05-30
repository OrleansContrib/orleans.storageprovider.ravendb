using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace Orleans.StorageProvider.RavenDB
{
    internal class EtagProvidingStoreListener : IDocumentStoreListener
    {
        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
        {
            return true;
        }

        public void AfterStore(string key, object entityInstance, RavenJObject metadata)
        {
            var grainState = entityInstance as IGrainState;
            if (grainState != null)
            {
                grainState.Etag = metadata.Value<string>("@etag");
            }
        }
    }
}