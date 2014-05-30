using Raven.Client.Document;

namespace Orleans.StorageProvider.RavenDB
{
    internal static class DocumentStoreExtensions
    {
        public static void RegisterNecessaryListeners(this DocumentStore store)
        {
            store
                .RegisterListener(new EtagProvidingConversionListener())
                .RegisterListener(new EtagProvidingStoreListener());
        }
    }
}