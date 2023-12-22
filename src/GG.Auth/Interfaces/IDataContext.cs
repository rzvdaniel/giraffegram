using Microsoft.EntityFrameworkCore;

namespace GG.Auth.Interfaces;

public interface IDataContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    void BeginTransaction();
    int Commit();
    void Rollback();
    Task<int> CommitAsync();
}
