using System;
using System.Threading.Tasks;
using Orleans.StorageProvider.RavenDB.TestInterfaces;

namespace Orleans.StorageProvider.RavenDB.Client
{
    /// <summary>
    /// Orleans test silo host
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // The Orleans silo environment is initialized in its own app domain in order to more
            // closely emulate the distributed situation, when the client and the server cannot
            // pass data via shared memory.
            AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = args,
            });

            Orleans.OrleansClient.Initialize("DevTestClientConfiguration.xml");

            var email = EmailFactory.GetGrain(1, "asdf@gmail.bs");
            email.Send().Wait();

            var grain = PersonFactory.GetGrain(1);
            var grain2 = PersonFactory.GetGrain(2);
            
            // If the name is set, we've run this code before.
            var name = grain.FirstName.Result;

            // We always override person 2
            var name2 = grain2.FirstName.Result;
            grain2.SetPersonalAttributes(new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male }).Wait();

            if (name != null)
            {
                Console.WriteLine("\n\nThis was found in the persistent store: {0}, {1}, {2}, {3}\n\n",
                    name,
                    grain.LastName.Result,
                    grain.Gender.Result,
                    grain.Age.Result);
            }
            else
            {
                grain.SetPersonalAttributes(new PersonalAttributes { FirstName = "John", LastName = "Doe", Age = 12, Gender = GenderType.Male }).Wait();
                Console.WriteLine("\n\nWe just wrote something to the persistent store. Please verify!\n\n");
            }

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadLine();

            hostDomain.DoCallBack(ShutdownSilo);
        }

        static void InitSilo(string[] args)
        {
            hostWrapper = new OrleansHostWrapper(args);

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }

        private static OrleansHostWrapper hostWrapper;
    }
}
