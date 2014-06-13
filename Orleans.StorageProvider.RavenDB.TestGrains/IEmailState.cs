using System;
using Orleans.StorageProvider.RavenDB.TestInterfaces;

namespace Orleans.StorageProvider.RavenDB.TestGrains
{
    public interface IEmailState : IGrainState
    {
        string Email { get; set; }
        DateTimeOffset? SentAt { get; set; }
        IPerson Person { get; set; }
    }
}