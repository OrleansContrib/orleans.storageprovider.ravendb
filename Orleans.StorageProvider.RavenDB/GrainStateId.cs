namespace Orleans.StorageProvider.RavenDB
{
    using System.Globalization;

    internal class GrainStateId
    {
        public GrainStateId(IGrainState grainState, GrainReference grainReference)
        {
            this.StateName = grainState.GetType().Name;
            this.Key = grainReference.ToKeyString();
            this.Id = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", this.StateName, this.Key);
        }

        public string StateName { get; private set; }

        public string Key { get; private set; }

        public string Id { get; private set; }

        public static implicit operator string(GrainStateId other)
        {
            return other.Id;
        }

        public override string ToString()
        {
            return this.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Id.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}