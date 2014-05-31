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
}
