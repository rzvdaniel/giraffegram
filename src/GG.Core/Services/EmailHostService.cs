using GG.Core.Dto;
using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GG.Core.Services;

public class EmailHostService(ApplicationDbContext dbContext, UserContextService userContextService)
{
    private readonly Guid userId = userContextService.GetUserId();

    public async Task<IEnumerable<EmailHost>> List()
    {
        return await dbContext.EmailHosts.Where(x => x.EmailHostUsers.Any(x => x.UserId == userId)).ToListAsync();
    }

    public EmailHost? Get(Guid id)
    {
        var entity = dbContext.EmailHosts.SingleOrDefault(x => x.Id == id && x.EmailHostUsers.Any(x => x.UserId == userId));

        return entity;
    }

    public async Task<Guid> Add(AddEmailHostDto emailHostDto, CancellationToken cancellationToken)
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

    public async Task<bool> Exists(string name, CancellationToken cancellationToken)
    {
        var hostExists = await dbContext.EmailHosts.AnyAsync(x => x.Name == name && x.EmailHostUsers.Any(x => x.UserId == userId));

        return hostExists;
    }
}
