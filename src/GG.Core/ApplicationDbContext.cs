using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GG.Core;

public class ApplicationDbContext : DbContext
{
    public DbSet<EmailAccount> EmailAccounts => Set<EmailAccount>();
    public DbSet<EmailAccountUser> EmailAccountUsers => Set<EmailAccountUser>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmailAccountUser>(entity =>
        {
            entity.HasKey(e => new { e.EmailAccountId, e.UserId });
            entity.HasIndex(e => e.EmailAccountId, "IX_.EmailHostUser_.EmailHostId");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.EmailAccountId).IsRequired();
        });
    }
}

