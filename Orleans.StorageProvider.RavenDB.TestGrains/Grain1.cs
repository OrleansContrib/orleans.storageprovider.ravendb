using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Orleans;
using Orleans.StorageProvider.RavenDB.TestInterfaces;

namespace Orleans.StorageProvider.RavenDB.TestGrains
{
    [StorageProvider(ProviderName = "RavenDBStorageProviderTestStore")]
    //[StorageProvider(ProviderName = "RavenDBStorageProviderLocalTestStore")]
    //[StorageProvider(ProviderName = "RavenDBStorageProviderInMemoryTestStore")]
    public class Person : GrainBase<IPersonState>, IPerson
    {
        Task IPerson.SetPersonalAttributes(PersonalAttributes props)
        {
            State.FirstName = props.FirstName;
            State.LastName = props.LastName;
            State.Gender = props.Gender;
            State.Age = props.Age;

            return State.WriteStateAsync();
        }

        Task<string> IPerson.FirstName
        {
            get { return Task.FromResult(State.FirstName); }
        }

        Task<string> IPerson.LastName
        {
            get { return Task.FromResult(State.LastName); }
        }

        Task<GenderType> IPerson.Gender
        {
            get { return Task.FromResult(State.Gender); }
        }

        Task<int> IPerson.Age
        {
            get { return Task.FromResult(State.Age); }
        }
    }

    [StorageProvider(ProviderName = "RavenDBStorageProviderTestStore")]
    public class Email : GrainBase<IEmailState>, IEmail
    {
        private string email;

        public override Task ActivateAsync()
        {
            this.GetPrimaryKey(out this.email);
            this.State.Email = this.email;

            return base.ActivateAsync();
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

		public Task<IPerson> Person
		{
			get { return Task.FromResult(State.Person); }
		}


		public Task SetPerson(IPerson person)
		{
			State.Person = person;
			return State.WriteStateAsync();
		}


		public Task<DateTimeOffset?> SentAt
		{
			get { return Task.FromResult(State.SentAt); }
		}
    }

    public interface IEmailState : IGrainState
    {
        string Email { get; set; }
        DateTimeOffset? SentAt { get; set; }
		IPerson Person { get; set; }
    }

    public interface IPersonState : IGrainState
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        GenderType Gender { get; set; }
        int Age { get; set; }
    }
}
