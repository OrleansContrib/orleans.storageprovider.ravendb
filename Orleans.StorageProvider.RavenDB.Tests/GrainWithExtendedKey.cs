namespace Orleans.StorageProvider.RavenDB.Tests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Orleans.StorageProvider.RavenDB.TestGrains;
    using Orleans.StorageProvider.RavenDB.TestInterfaces;
    using Orleans.StorageProvider.RavenDB.Tests.Infrastructure;
    using Raven.Client;

    [TestFixture]
    public class GrainWithExtendedKey
    {
        private const string EmailId = "EmailState/0000000000000000000000000000000106ffffffc4b12014+asdf@gmail.bs";

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
            var email = EmailFactory.GetGrain(1, "asdf@gmail.bs");
            await email.Send();

            IAsyncDocumentSession session = this.ravenSetup.NewAsyncSession();
            var actualStored = await session.LoadAsync<IEmailState>(EmailId);

            actualStored.Email.Should().Be("asdf@gmail.bs");
            actualStored.SentAt.Should().HaveValue();
            actualStored.Person.Should().BeNull();
        }
    }
}