namespace Orleans.StorageProvider.RavenDB.TestGrains
{
    using System.Threading.Tasks;
    using Orleans.Providers;
    using Orleans.StorageProvider.RavenDB.TestInterfaces;

    [StorageProvider(ProviderName = "RavenDBStorageProviderTests")]
    public class Person : Grain<IPersonState>, IPerson
    {
        Task<string> IPerson.GetFirstName()
        {
            return Task.FromResult(State.FirstName);
        }

        Task<string> IPerson.GetLastName()
        {
            return Task.FromResult(State.LastName);
        }

        Task<GenderType> IPerson.GetGender()
        {
             return Task.FromResult(State.Gender);
        }

        Task<int> IPerson.GetAge()
        {
            return Task.FromResult(State.Age);
        }

        Task IPerson.SetPersonalAttributes(PersonalAttributes props)
        {
            State.FirstName = props.FirstName;
            State.LastName = props.LastName;
            State.Gender = props.Gender;
            State.Age = props.Age;

            return State.WriteStateAsync();
        }
    }
}
