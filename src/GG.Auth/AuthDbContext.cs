using GG.Auth.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GG.Auth;

public class AuthDbContext :
    IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<ClientUser> ClientUsers => Set<ClientUser>();

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClientUser>(entity =>
        {
            entity.HasKey(e => new { e.ClientId, e.UserId });
            entity.HasIndex(e => e.ClientId, "IX_ClientUser_ClientId");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.ClientId).IsRequired();
        });
    }
}
