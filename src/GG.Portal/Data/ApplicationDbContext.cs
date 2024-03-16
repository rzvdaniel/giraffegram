using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GG.Portal.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<EmailAccountEntity> EmailAccounts => Set<EmailAccountEntity>();
        public DbSet<EmailAccountUserEntity> EmailAccountUsers => Set<EmailAccountUserEntity>();
        public DbSet<AppEmailTemplateEntity> AppEmailTemplates => Set<AppEmailTemplateEntity>();
        public DbSet<EmailTemplateEntity> EmailTemplates => Set<EmailTemplateEntity>();
        public DbSet<EmailTemplateUserEntity> EmailTemplateUsers => Set<EmailTemplateUserEntity>();
        public DbSet<ApiKeyEntity> ApiKeys => Set<ApiKeyEntity>();
        public DbSet<ApiKeyUserEntity> ApiKeyUsers => Set<ApiKeyUserEntity>();
        public DbSet<LogEntity> Logs => Set<LogEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailAccountUserEntity>(entity =>
            {
                entity.HasKey(e => new { e.EmailAccountId, e.UserId });
                entity.HasIndex(e => e.EmailAccountId, "IX_EmailAccountUser_EmailAccountId");
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.EmailAccountId).IsRequired();
            });

            modelBuilder.Entity<EmailTemplateUserEntity>(entity =>
            {
                entity.HasKey(e => new { e.EmailTemplateId, e.UserId });
                entity.HasIndex(e => e.EmailTemplateId, "IX_EmailTemplateUser_EmailTemplateId");
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.EmailTemplateId).IsRequired();
            });

            modelBuilder.Entity<ApiKeyUserEntity>(entity =>
            {
                entity.HasKey(e => new { e.ApiKeyId, e.UserId });
                entity.HasIndex(e => e.ApiKeyId, "IX_ApiKeyUser_ApiKeyId");
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.ApiKeyId).IsRequired();
            });
        }
    }
}
