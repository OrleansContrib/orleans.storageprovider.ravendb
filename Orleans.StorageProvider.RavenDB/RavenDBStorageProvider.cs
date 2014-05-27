namespace Orleans.StorageProvider.RavenDB
{
    using System.Configuration;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Providers;
    using Storage;
    using Raven.Abstractions.Commands;
    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Embedded;
    using Raven.Database.Server;
    using Raven.Json.Linq;

    /// <summary>
    /// A RavenDB storage provider.
    /// </summary>
    /// <remarks>
    /// The storage provider should be included in a deployment by adding this line to the Orleans server configuration file:
    /// <Provider Type="Orleans.StorageProvider.RavenDB.RavenDBStorageProvider" Name="RavenDBStore" ConnectionStringName="RavenDB"/>and this line to any grain that uses it:
    /// [StorageProvider(ProviderName = "RavenDBStore")]
    /// The name 'RavenDBStore' is an arbitrary choice.
    /// If no connection string name is provided the provider will use RavenDB InMemory storage.
    /// </remarks>
    public class RavenDBStorageProvider : IStorageProvider
    {
        private DocumentStore documentStore;
        public string Name { get; private set; }

        public OrleansLogger Log { get; private set; }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            this.Log = providerRuntime.GetLogger(this.GetType().FullName, Logger.LoggerType.Application);

            string connectionStringName = config.Properties["ConnectionStringName"];

            if (string.IsNullOrEmpty(connectionStringName))
            {
                this.Log.Info("Starting RavenDB Storage Provider InMemory");
                return this.InMemoryMode();
            }

            var settings = ConfigurationManager.ConnectionStrings[connectionStringName];

            var connectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = settings.ConnectionString
            };

            object url;
            if (connectionStringBuilder.TryGetValue("Url", out url))
            {
                this.Log.Info("Starting RavenDB Storage Provider attached to server {0}", url);
                return this.ServerMode(connectionStringName);
            }

            object dataDir;
            if (connectionStringBuilder.TryGetValue("DataDir", out dataDir))
            {
                this.Log.Info("Starting RavenDB Storage Provider embedded in directory {0}", dataDir);
                return this.LocalMode(connectionStringName);
            }

            return TaskDone.Done;
        }

        public Task Close()
        {
            return Task.Run(() => this.documentStore.Dispose());
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var stateName = grainState.GetType().Name;
            var key = grainReference.ToKeyString();
            var id = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", stateName, key);

            using (IAsyncDocumentSession session = this.documentStore.OpenAsyncSession())
            {
                var state = await session.LoadAsync<RavenJObject>(id);
                if (state != null)
                {
                    grainState.SetAll(state.ToDictionary(x => x.Key, x => x.Value.Value<object>()));
                }
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var stateName = grainState.GetType().Name;
            var key = grainReference.ToKeyString();
            var id = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", stateName, key);

            using (IAsyncDocumentSession session = this.documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(grainState, id);
                await session.SaveChangesAsync();
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, GrainState grainState)
        {
            var stateName = grainState.GetType().Name;
            var key = grainReference.ToKeyString();
            var id = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", stateName, key);

            using (IAsyncDocumentSession session = this.documentStore.OpenAsyncSession())
            {
                session.Advanced.Defer(new DeleteCommandData
                {
                    Key = id
                });
                await session.SaveChangesAsync();
            }
        }

        private Task ServerMode(string connectionStringName)
        {
            return Task.Run(() =>
            {
                this.documentStore = new DocumentStore
                {
                    ConnectionStringName = connectionStringName,
                };
                this.documentStore.Initialize();
            });
        }

        private Task InMemoryMode()
        {
            return Task.Run(() =>
            {
                this.documentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };

                this.documentStore.Initialize();
            });
        }

        private Task LocalMode(string connectionStringName)
        {
            return Task.Run(() =>
            {
                this.documentStore = new EmbeddableDocumentStore
                {
                    ConnectionStringName = connectionStringName,
                    UseEmbeddedHttpServer = true
                };

                NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);

                this.documentStore.Initialize();
            });
        }
    }
}