using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Raven.Imports.Newtonsoft.Json.Utilities;

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
        private static ConcurrentDictionary<Type, MethodInfo> loadAsyncMethodInfoCache = new ConcurrentDictionary<Type, MethodInfo>(); 
        private DocumentStore documentStore;
        public string Name { get; private set; }

        public OrleansLogger Log { get; private set; }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            this.Name = name;
            this.Log = providerRuntime.GetLogger("Storage.RavenDB", Logger.LoggerType.Runtime);

            string connectionStringName = config.Properties["ConnectionStringName"];
            
            if (string.IsNullOrEmpty(connectionStringName))
            {
                this.Log.Info("Init: Name={0} Mode=InMemory", this.Name);
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
                this.Log.Info("Init: Name={0} Mode=Server Url={1} ConnectionString={2}", this.Name, url, connectionStringBuilder.ConnectionString);
                return this.ServerMode(connectionStringName);
            }

            object dataDir;
            if (connectionStringBuilder.TryGetValue("DataDir", out dataDir))
            {
                this.Log.Info("Init: Name={0} Mode=Embedded DataDir={1} ConnectionString={2}", this.Name, dataDir, connectionStringBuilder.ConnectionString);
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

            this.Log.Verbose3("Reading: GrainType={0} Pk={1} Grainid={2} from Document={3}", grainType, key, grainReference, id);

            using (IAsyncDocumentSession session = this.documentStore.OpenAsyncSession())
            {
                var methodInfo = loadAsyncMethodInfoCache.GetOrAdd(
                    grainState.GetType(), 
                    t => session.GetType().GetGenericMethod("LoadAsync", typeof(string)).MakeGenericMethod(grainState.GetType()));
                var state = await((dynamic)methodInfo.Invoke(session, new object[] { id }));
                if (state != null)
                {
                    this.Log.Verbose3("Read: GrainType={0} Pk={1} Grainid={2} from Document={3}", grainType, key, grainReference, id);
                    grainState.SetAll(((IGrainState)state).AsDictionary());
                }
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            var stateName = grainState.GetType().Name;
            var key = grainReference.ToKeyString();
            var id = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", stateName, key);

            this.Log.Verbose3("Writing: GrainType={0} Pk={1} Grainid={2} ETag={3} to Document={4}", grainType, key, grainReference, grainState.Etag, id);

            using (IAsyncDocumentSession session = this.documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(grainState, id);
                await session.SaveChangesAsync();

                this.Log.Verbose3("Written: GrainType={0} Pk={1} Grainid={2} ETag={3} to Document={4}", grainType, key, grainReference, grainState.Etag, id);
            }
        }

        public async Task ClearStateAsync(string grainType, GrainReference grainReference, GrainState grainState)
        {
            var stateName = grainState.GetType().Name;
            var key = grainReference.ToKeyString();
            var id = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", stateName, key);

            this.Log.Verbose3("Clearing: GrainType={0} Pk={1} Grainid={2} ETag={3} from Document={4}", grainType, key, grainReference, grainState.Etag, id);

            using (IAsyncDocumentSession session = this.documentStore.OpenAsyncSession())
            {
                session.Advanced.Defer(new DeleteCommandData
                {
                    Key = id
                });
                await session.SaveChangesAsync();

                this.Log.Verbose3("Cleared: GrainType={0} Pk={1} Grainid={2} ETag={3} from Document={4}", grainType, key, grainReference, grainState.Etag, id);
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
                    UseEmbeddedHttpServer = true,
                };

                NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8080);

                this.documentStore.Initialize();
            });
        }
    }
}