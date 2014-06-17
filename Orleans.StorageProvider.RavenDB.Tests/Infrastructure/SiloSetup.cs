namespace Orleans.StorageProvider.RavenDB.Tests.Infrastructure
{
    using System;
    using System.Diagnostics;
    using Orleans.Samples.Testing;

    public class SiloSetup : IDisposable
    {
        private static readonly UnitTestSiloOptions SiloOptions = new UnitTestSiloOptions
        {
            StartFreshOrleans = true,
            StartSecondary = false,
        };

        private static readonly UnitTestClientOptions ClientOptions = new UnitTestClientOptions
        {
            ResponseTimeout = TimeSpan.FromSeconds(30)
        };

        private readonly UnitTestSiloHost unitTestSiloHost;

        public SiloSetup()
        {
            this.unitTestSiloHost = new UnitTestSiloHost(SiloOptions, ClientOptions);
        }

        public void Dispose()
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
    }
}