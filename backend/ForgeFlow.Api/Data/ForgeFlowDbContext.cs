using ForgeFlow.Api.Data.Entities;
using ForgeFlow.Api.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForgeFlow.Api.Data;

public class ForgeFlowDbContext(DbContextOptions<ForgeFlowDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Bucket> Buckets => Set<Bucket>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Bucket>(entity =>
        {
            entity.HasKey(bucket => bucket.BucketKey);

            entity.Property(bucket => bucket.BucketKey)
                .HasMaxLength(Bucket.BucketKeyMaxLength);
        });
    }
}
