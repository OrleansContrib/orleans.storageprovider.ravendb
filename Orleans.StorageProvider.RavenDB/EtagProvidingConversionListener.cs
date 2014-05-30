using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace Orleans.StorageProvider.RavenDB
{
    internal class EtagProvidingConversionListener : IDocumentConversionListener
    {
        public void EntityToDocument(string key, object entity, RavenJObject document, RavenJObject metadata)
        {
            // We don't want to store the Etag in the document because this is taken care of by RavenDB metadata
            document.Remove("Etag");
        }

        public void DocumentToEntity(string key, object entity, RavenJObject document, RavenJObject metadata)
        {
            var grainState = entity as IGrainState;
            if (grainState != null)
            {
                grainState.Etag = metadata.Value<string>("@etag");
            }
        }
    }
}