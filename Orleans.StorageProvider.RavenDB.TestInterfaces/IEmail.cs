using System.Threading.Tasks;

namespace Orleans.StorageProvider.RavenDB.TestInterfaces
{
    [ExtendedPrimaryKey]
    public interface IEmail : IGrain
    {
        Task Send();
    }
}