namespace Orleans.StorageProvider.RavenDB.TestGrains
{
    using System;
    using System.Threading.Tasks;
    using Orleans.Providers;
    using Orleans.StorageProvider.RavenDB.TestInterfaces;

    [StorageProvider(ProviderName = "RavenDBStorageProviderTests")]
    public class Email : Grain<IEmailState>, IEmail
    {
        private string email;

        public Task<IPerson> GetPerson()
        {
             return Task.FromResult(State.Person);
        }


        public Task<DateTimeOffset?> GetSentAt()
        {
             return Task.FromResult(State.SentAt);
        }

        public async Task Send()
        {
            if (this.State.SentAt.HasValue)
            {
                return;
            }

            this.State.SentAt = DateTimeOffset.UtcNow;

            await this.State.WriteStateAsync();
        }

        public async Task SetPerson(IPerson person)
        {
            State.Person = person;

            await this.State.WriteStateAsync();
        }

        public override Task ActivateAsync()
        {
            this.GetPrimaryKey(out this.email);
            this.State.Email = this.email;

            return base.ActivateAsync();
        }
    }
}