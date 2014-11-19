namespace Orleans.StorageProvider.RavenDB.TestGrains
{
    using System;
    using Orleans.StorageProvider.RavenDB.TestInterfaces;

    public interface IEmailState : IGrainState
    {
        string Email { get; set; }
        DateTimeOffset? SentAt { get; set; }
        IPerson Person { get; set; }
    }
}