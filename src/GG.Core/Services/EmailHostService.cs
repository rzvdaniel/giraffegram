using GG.Core.Dto;
using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GG.Core.Services;

public class EmailHostService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<EmailHost>> List(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.EmailHosts.Where(x => x.EmailHostUsers.Any(x => x.UserId == userId)).ToListAsync();
    }

    public async Task<EmailHost?> Get(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EmailHosts.SingleOrDefaultAsync(x => x.Id == id && x.EmailHostUsers.Any(x => x.UserId == userId), cancellationToken);

        return entity;
    }

    public async Task<EmailHost?> Get(string hostName, Guid userId, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EmailHosts.SingleOrDefaultAsync(x => x.Name == hostName && x.EmailHostUsers.Any(x => x.UserId == userId), cancellationToken);

        return entity;
    }

    public async Task<Guid> Add(AddEmailHostDto emailHostDto, Guid userId, CancellationToken cancellationToken)
    {
        var emailHost = new EmailHost
        {
            Id = Guid.NewGuid(),
            Name = emailHostDto.Name,
            Host = emailHostDto.Host,
            Port = emailHostDto.Port,
            UseSsl = emailHostDto.UseSsl,
            UserName = emailHostDto.UserName,
            UserPassword = emailHostDto.UserPassword,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.EmailHosts.Add(emailHost);

        var emailHostUser = new EmailHostUser
        {
            EmailHostId = emailHost.Id,
            UserId = userId
        };

        dbContext.EmailHostUsers.Add(emailHostUser);

        await dbContext.SaveChangesAsync(cancellationToken);

        return emailHost.Id;
    }

    public async Task<bool> Exists(string name, Guid userId, CancellationToken cancellationToken)
    {
        var hostExists = await dbContext.EmailHosts.AnyAsync(x => x.Name == name && x.EmailHostUsers.Any(x => x.UserId == userId), cancellationToken);

        return hostExists;
    }
}
