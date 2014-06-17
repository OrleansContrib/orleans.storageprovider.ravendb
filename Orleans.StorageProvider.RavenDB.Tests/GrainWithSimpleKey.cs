namespace Orleans.StorageProvider.RavenDB.Tests
{
    using System.Dynamic;
    using FluentAssertions;
    using NUnit.Framework;
    using Orleans.StorageProvider.RavenDB.TestGrains;
    using Orleans.StorageProvider.RavenDB.TestInterfaces;
    using Orleans.StorageProvider.RavenDB.Tests.Infrastructure;
    using Raven.Client;

    [TestFixture]
    public class GrainWithSimpleKey
    {
        private const string PersonId = "PersonState/0000000000000000000000000000000103ffffffd411bc7a";

        private RavenSetup ravenSetup;
        private SiloSetup siloSetup;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            this.ravenSetup = new RavenSetup();
            this.siloSetup = new SiloSetup();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            this.siloSetup.Dispose();
            this.ravenSetup.Dispose();
        }

        [Test]
        public async void LoadedSuccessful()
        {
            var expected = new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male };

            var person = PersonFactory.GetGrain(1);
            await person.SetPersonalAttributes(expected);

            var result = PersonFactory.GetGrain(1);
            var actual = new PersonalAttributes
            {
                FirstName = await result.FirstName,
                LastName = await result.LastName,
                Age = await result.Age,
                Gender = await result.Gender
            };

            IAsyncDocumentSession session = this.ravenSetup.NewAsyncSession();
            var actualStored = await session.LoadAsync<IPersonState>(PersonId);

            actual.ShouldBeEquivalentTo(expected);
            actualStored.ShouldBeEquivalentTo(expected, options => options.ExcludingMissingProperties());
        }
    }
}