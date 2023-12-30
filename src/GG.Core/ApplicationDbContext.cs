using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GG.Core;

public class ApplicationDbContext : DbContext
{
    public DbSet<EmailHost> EmailHosts => Set<EmailHost>();
    public DbSet<EmailHostUser> EmailHostUsers => Set<EmailHostUser>();

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

        modelBuilder.Entity<EmailHostUser>(entity =>
        {
            entity.HasKey(e => new { e.EmailHostId, e.UserId });
            entity.HasIndex(e => e.EmailHostId, "IX_.EmailHostUser_.EmailHostId");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.EmailHostId).IsRequired();
        });
    }
}

