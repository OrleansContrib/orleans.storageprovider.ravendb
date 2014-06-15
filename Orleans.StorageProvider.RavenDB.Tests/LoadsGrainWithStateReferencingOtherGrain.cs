using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Orleans.Samples.Testing;
using Orleans.StorageProvider.RavenDB.TestInterfaces;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace Orleans.StorageProvider.RavenDB.Tests
{
    [TestFixture]
    public class LoadsGrainWithStateReferencingOtherGrain
    {
        private static readonly UnitTestSiloOptions siloOptions = new UnitTestSiloOptions
        {
            StartFreshOrleans = true,
            StartSecondary = false,
        };

        private static readonly UnitTestClientOptions clientOptions = new UnitTestClientOptions
        {
            ResponseTimeout = TimeSpan.FromSeconds(30)
        };

        private UnitTestSiloHost unitTestSiloHost;
        private DocumentStore documentStore;
        private IAsyncDocumentSession session;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            this.documentStore = new DocumentStore
            {
                ConnectionStringName = "RavenDB",
                DefaultDatabase = "RavenDBStorageProviderTests"
            };
            this.documentStore.Initialize();
            this.documentStore.DatabaseCommands.EnsureDatabaseExists("RavenDBStorageProviderTests");
            this.session = this.documentStore.OpenAsyncSession();

            this.unitTestSiloHost = new UnitTestSiloHost(siloOptions, clientOptions);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            if (this.unitTestSiloHost == null)
            {
                return;
            }

            try
            {
                this.unitTestSiloHost.StopDefaultSilos();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }
        }

        [Test]
        public async void WriteGrainInGrainToStoreFirst()
        {
            var person = PersonFactory.GetGrain(1);
            await person.SetPersonalAttributes(new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male });

            var email = EmailFactory.GetGrain(1, "asdf@gmail.bs");
            await email.SetPerson(person);
            await email.Send();
        }

        [Test]
        public async void WriteGrainInGrainToStoreSecond()
        {
            var emailAgain = EmailFactory.GetGrain(1, "asdf@gmail.bs");
            var expected = await emailAgain.Person;
            Assert.NotNull(expected);
        }
    }
}