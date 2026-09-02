namespace ForgeFlow.Api.Models;

// Chosen when the bucket is created and permanent afterwards.
public enum BucketRetention
{
    Transient,   // objects deleted after 24 hours
    Temporary,   // deleted after 30 days
    Persistent,  // kept until deleted
}
