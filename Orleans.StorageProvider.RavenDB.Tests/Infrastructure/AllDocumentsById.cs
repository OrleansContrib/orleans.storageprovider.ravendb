namespace Orleans.StorageProvider.RavenDB.Tests.Infrastructure
{
    using System.Linq;
    using Raven.Client.Indexes;

    public class AllDocumentsById : AbstractIndexCreationTask<object>
    {
        public AllDocumentsById()
        {
            this.Map = docs => from doc in docs select new { Id = this.MetadataFor(doc)["@id"] };
        }

        public override string IndexName
        {
            get
            {
                return "AllDocumentsById";
            }
        }
    }
}