namespace Orleans.StorageProvider.RavenDB
{
    using Orleans.Serialization.RavenDB.Json;
    using Raven.Client;

    internal static class DocumentStoreExtensions
    {
        public static void RegisterDocumentStoreCustomizations(this DocumentStoreBase store)
        {
            store.ConfigureContractResolver();

            store
                .RegisterListener(new EtagProvidingConversionListener())
                .RegisterListener(new EtagProvidingStoreListener());
        }
    }
}