using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Orleans;

namespace Orleans.StorageProvider.RavenDB.TestInterfaces
{
    /// <summary>
    /// Orleans grain communication interface IPerson
    /// </summary>
    public interface IPerson : IGrain
    {
        Task SetPersonalAttributes(PersonalAttributes person);

        Task<string> FirstName { get; }
        Task<string> LastName { get; }
        Task<int> Age { get; }
        Task<GenderType> Gender { get; }
    }

    public enum GenderType { Male, Female }

    [Serializable]
    public class PersonalAttributes
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public GenderType Gender { get; set; }
    }

    [ExtendedPrimaryKey]
    public interface IEmail : IGrain
    {
        Task Send();
    }
}
