using GG.ComingSoon.Core;
using Microsoft.EntityFrameworkCore;

namespace GG.ComingSoon.Api;

public class EmailSubscriptionDbContext : DbContext
{
    public DbSet<EmailSubscription> EmailSubscriptions => Set<EmailSubscription>();

    public EmailSubscriptionDbContext(DbContextOptions<EmailSubscriptionDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
