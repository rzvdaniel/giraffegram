using GG.Auth.Entities;
using GG.Auth.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GG.Auth;

public class ApplicationDbContext : 
    IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IDataContext
{
    private IDbContextTransaction? transaction;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public void BeginTransaction()
    {
        transaction = Database.BeginTransaction();
    }

    public int Commit()
    {
        try
        {
            var saveChanges = SaveChanges();
            transaction?.Commit();
            return saveChanges;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    public void Rollback()
    {
        transaction?.Rollback();
        transaction?.Dispose();
    }

    public Task<int> CommitAsync()
    {
        try
        {
            var saveChangesAsync = SaveChangesAsync();
            transaction?.Commit();
            return saveChangesAsync;
        }
        finally
        {
            transaction?.Dispose();
        }
    }
}
