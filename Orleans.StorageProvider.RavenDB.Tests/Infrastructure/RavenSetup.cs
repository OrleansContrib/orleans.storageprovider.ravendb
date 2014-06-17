namespace Orleans.StorageProvider.RavenDB.Tests.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Raven.Abstractions.Data;
    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Extensions;
    using Raven.Client.Indexes;

    public class RavenSetup : IDisposable
    {
        private readonly DocumentStoreBase documentStore;

        private readonly List<IDisposable> sessions;

        public RavenSetup()
        {
            this.sessions = new List<IDisposable>();

            this.documentStore = new DocumentStore
            {
                ConnectionStringName = "RavenDB",
                DefaultDatabase = "RavenDBStorageProviderTests",
                Conventions =
                {
                    // This forces raven db to always wait for non stale results. This is useful for unit testing with queries.
                    DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite
                }
            };
            this.documentStore.Initialize();
            this.documentStore.DatabaseCommands.EnsureDatabaseExists("RavenDBStorageProviderTests");

            // we need the same conventions as in production.
            this.documentStore.RegisterDocumentStoreCustomizations();

            IndexCreation.CreateIndexes(this.GetType().Assembly, this.DocumentStore);

            this.Clear();
        }

        public IDocumentStore DocumentStore
        {
            get
            {
                return this.documentStore;
            }
        }

        /// <summary>
        /// Creates a new session and tracks it. The created session is automatically disposed.
        /// </summary>
        public IDocumentSession NewSession()
        {
            var session = this.documentStore.OpenSession();
            this.sessions.Add(session);
            return session;
        }

        /// <summary>
        /// Creates a new async session and tracks it. The created session is automatically disposed.
        /// </summary>
        public IAsyncDocumentSession NewAsyncSession()
        {
            var session = this.documentStore.OpenAsyncSession();
            this.sessions.Add(session);
            return session;
        }

        public void WaitForIndexing()
        {
            SpinWait.SpinUntil(() => this.DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length == 0);
        }

        public void Clear()
        {
            this.WaitForIndexing();

            this.DocumentStore.DatabaseCommands.DeleteByIndex("AllDocumentsById", new IndexQuery());

            this.ClearSessions();
        }

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearSessions();

                this.documentStore.Dispose();
            }
        }

        private void ClearSessions()
        {
            foreach (var session in this.sessions)
            {
                session.Dispose();
            }

            this.sessions.Clear();
        }
    }
}