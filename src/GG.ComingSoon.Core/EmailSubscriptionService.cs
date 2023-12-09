using GG.ComingSoon.Api;
using GG.ComingSoon.Core.Dto;
using Microsoft.EntityFrameworkCore;

namespace GG.ComingSoon.Core;

public class EmailSubscriptionService(EmailSubscriptionDbContext dbContext)
{
    public async Task<IEnumerable<EmailSubscription>> List()
    {
        return await dbContext.EmailSubscriptions.ToListAsync();
    }

    public async Task Add(AddEmailSubscriptionDto emailSubscriptionDto, CancellationToken cancellationToken)
    {
        var emailSubscription = new EmailSubscription
        {
            Id = Guid.NewGuid(),
            Email = emailSubscriptionDto.Email,
            SubscribedAt = DateTime.UtcNow
        };

        dbContext.EmailSubscriptions.Add(emailSubscription);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> Exists(string email, CancellationToken cancellationToken)
    {
        var emailExists = await dbContext.EmailSubscriptions.AnyAsync(x => x.Email == email, cancellationToken);

        return emailExists;
    }
}
