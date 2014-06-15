using Raven.Client.Document;

namespace Orleans.StorageProvider.RavenDB
{
    internal static class DocumentStoreExtensions
    {
        public static void RegisterDocumentStoreCustomizations(this DocumentStore store)
        {
            store.Conventions.JsonContractResolver = new GrainReferenceAwareContractResolver();

            store
                .RegisterListener(new EtagProvidingConversionListener())
                .RegisterListener(new EtagProvidingStoreListener());
        }
    }
}