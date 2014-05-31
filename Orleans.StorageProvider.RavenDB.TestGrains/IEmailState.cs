using System;

namespace Orleans.StorageProvider.RavenDB.TestGrains
{
    public interface IEmailState : IGrainState
    {
        string Email { get; set; }
        DateTimeOffset? SentAt { get; set; }
    }
}