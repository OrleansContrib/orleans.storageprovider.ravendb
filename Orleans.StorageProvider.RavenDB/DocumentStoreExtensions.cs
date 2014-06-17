namespace Orleans.StorageProvider.RavenDB
{
    using Raven.Client;

    internal static class DocumentStoreExtensions
    {
        public static void RegisterDocumentStoreCustomizations(this DocumentStoreBase store)
        {
            store.Conventions.JsonContractResolver = new GrainReferenceAwareContractResolver();

            store
                .RegisterListener(new EtagProvidingConversionListener())
                .RegisterListener(new EtagProvidingStoreListener());
        }
    }
}