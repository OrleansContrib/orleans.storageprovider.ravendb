using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans.StorageProvider.RavenDB.TestInterfaces;

namespace Orleans.StorageProvider.RavenDb.Tests
{
	[TestClass]
	public class RavenDbStorageProviderTest
	{
		[TestInitialize]
		public void StartSilo()
		{
			OrleansClient.Initialize("DevTestClientConfiguration.xml");
		}

		[TestCleanup]
		public void CloseSilo()
		{
			OrleansClient.Uninitialize();
		}

		[TestMethod]
		async public Task GetGrainNeverSeenBeforeAndWriteToStore()
		{
			Guid newGrainId = Guid.NewGuid();

			var grain = EmailFactory.GetGrain(newGrainId, "asdf@gmail.bs#ç%&/()+¦");

			var expected = await grain.SentAt;

			grain.Send().Wait();

			var actual = await grain.SentAt;

			Assert.AreNotEqual<DateTimeOffset?>(expected, actual);
		}

		[TestMethod]
		async public Task GetGrainsAndWriteToStore()
		{
			var expected = PersonFactory.GetGrain(2);

			expected.SetPersonalAttributes(new PersonalAttributes { FirstName = "John Copy", LastName = "Doe Copy", Age = 24, Gender = GenderType.Male }).Wait();

			var actual = PersonFactory.GetGrain(2);

			Assert.AreEqual(24, await actual.Age);
			Assert.AreEqual(GenderType.Male, await actual.Gender);
			Assert.AreEqual("John Copy", await actual.FirstName);
			Assert.AreEqual("Doe Copy", await actual.LastName);
		}

		[TestMethod]
		public void WriteGrainInGrainToStore()
		{
			var grain = PersonFactory.GetGrain(1);

			var email = EmailFactory.GetGrain(1, "asdf@gmail.bs#ç%&/()+¦");
			email.SetPerson(grain);
			email.Send().Wait();

		}

	}
}
