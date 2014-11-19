using System.Threading.Tasks;

namespace Orleans.StorageProvider.RavenDB.TestInterfaces
{
    /// <summary>
    /// Orleans grain communication interface IPerson
    /// </summary>
    public interface IPerson : IGrain
    {
        Task SetPersonalAttributes(PersonalAttributes person);

        Task<string> GetFirstName();
        Task<string> GetLastName();
        Task<int> GetAge();
        Task<GenderType> GetGender();
    }
}