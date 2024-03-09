using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GG.Core;

public class ApplicationDbContext : DbContext
{
    public DbSet<EmailAccount> EmailAccounts => Set<EmailAccount>();
    public DbSet<EmailAccountUser> EmailAccountUsers => Set<EmailAccountUser>();
    public DbSet<AppEmailTemplate> AppEmailTemplates => Set<AppEmailTemplate>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<EmailTemplateUser> EmailTemplateUsers => Set<EmailTemplateUser>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<ApiKeyUser> ApiKeyUsers => Set<ApiKeyUser>();
    public DbSet<Log> Logs => Set<Log>();

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
            entity.HasIndex(e => e.EmailAccountId, "IX_EmailAccountUser_EmailAccountId");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.EmailAccountId).IsRequired();
        });

        modelBuilder.Entity<EmailTemplateUser>(entity =>
        {
            entity.HasKey(e => new { e.EmailTemplateId, e.UserId });
            entity.HasIndex(e => e.EmailTemplateId, "IX_EmailTemplateUser_EmailTemplateId");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.EmailTemplateId).IsRequired();
        });

        modelBuilder.Entity<ApiKeyUser>(entity =>
        {
            entity.HasKey(e => new { e.ApiKeyId, e.UserId });
            entity.HasIndex(e => e.ApiKeyId, "IX_ApiKeyUser_ApiKeyId");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.ApiKeyId).IsRequired();
        });
    }
}

