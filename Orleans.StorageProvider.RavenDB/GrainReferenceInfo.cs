namespace Orleans.StorageProvider.RavenDB
{
    internal class GrainReferenceInfo
    {
        public string Key { get; set; }

        public byte[] Data { get; set; }
    }
}