using GG.Portal.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GG.Portal.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, UserRole, Guid>(options)
    {
        public DbSet<EmailAccount> EmailAccounts => Set<EmailAccount>();
        public DbSet<EmailAccountUser> EmailAccountUsers => Set<EmailAccountUser>();
        public DbSet<AppEmailTemplate> AppEmailTemplates => Set<AppEmailTemplate>();
        public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
        public DbSet<EmailTemplateUser> EmailTemplateUsers => Set<EmailTemplateUser>();
        public DbSet<ApiKeyEntity> ApiKeys => Set<ApiKeyEntity>();
        public DbSet<ApiKeyUser> ApiKeyUsers => Set<ApiKeyUser>();
        public DbSet<Log> Logs => Set<Log>();

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
}
