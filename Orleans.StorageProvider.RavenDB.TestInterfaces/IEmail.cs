namespace Orleans.StorageProvider.RavenDB.TestInterfaces
{
    using System.Threading.Tasks;

    [ExtendedPrimaryKey]
    public interface IEmail : IGrain
    {
        Task SetPerson(IPerson person);
        Task<IPerson> GetPerson();
        Task Send();
    }
}