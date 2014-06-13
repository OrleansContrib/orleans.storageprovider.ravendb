using System;
using System.Diagnostics;
using NUnit.Framework;
using Orleans.Samples.Testing;
using Orleans.StorageProvider.RavenDB.TestInterfaces;

namespace Orleans.StorageProvider.RavenDB.Tests
{
    [TestFixture]
    public class LoadsGrainWithStateReferencingOtherGrain
    {
        private static readonly UnitTestSiloOptions siloOptions = new UnitTestSiloOptions
        {
            StartFreshOrleans = true,
        };

        private static readonly UnitTestClientOptions clientOptions = new UnitTestClientOptions
        {
            ResponseTimeout = TimeSpan.FromSeconds(30)
        };

        private UnitTestSiloHost unitTestSiloHost;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
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
        public async void WriteGrainInGrainToStore()
        {
            var expected = PersonFactory.GetGrain(1);
            await expected.SetPersonalAttributes(new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male });

            var email = EmailFactory.GetGrain(1, "asdf@gmail.bs#ç%&/()+¦");
            await email.SetPerson(expected);
            await email.Send();
        }
    }
}