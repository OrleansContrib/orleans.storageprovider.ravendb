namespace Orleans.StorageProvider.RavenDB.Tests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Orleans.StorageProvider.RavenDB.TestInterfaces;
    using Orleans.StorageProvider.RavenDB.Tests.Infrastructure;

    [TestFixture]
    public class GrainWithStateReferencingOtherGrain
    {
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
            var person = PersonFactory.GetGrain(2);
            await person.SetPersonalAttributes(new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male });

            var email = EmailFactory.GetGrain(2, "asdf@gmail.bs");
            await email.SetPerson(person);
            await email.Send();

            var emailAgain = EmailFactory.GetGrain(2, "asdf@gmail.bs");
            var expectedPerson = await emailAgain.GetPerson();

            expectedPerson.Should().NotBeNull();
        }
    }
}