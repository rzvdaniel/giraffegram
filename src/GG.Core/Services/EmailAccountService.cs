using GG.Core.Dto;
using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GG.Core.Services;

public class EmailAccountService(ApplicationDbContext dbContext, SecretKeyEncryptionService encryptionService, AppConfigService appConfigService)
{
    public async Task<IEnumerable<EmailAccount>> List(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.EmailAccounts.Where(x => x.EmailAccountUsers.Any(x => x.UserId == userId)).ToListAsync(cancellationToken);
    }

    public async Task<EmailAccount?> Get(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var emailAccount = await dbContext.EmailAccounts.SingleOrDefaultAsync(x => x.Id == id && x.EmailAccountUsers.Any(x => x.UserId == userId), cancellationToken);

        if (emailAccount != null)
        {
            var decryptedPassword = await encryptionService.DecryptAsync(Encoding.Unicode.GetBytes(emailAccount.UserPassword), appConfigService.UserEncryptionKey);
            emailAccount.UserPassword = decryptedPassword;
        }

        return emailAccount;
    }

    public async Task<EmailAccount?> Get(string hostName, Guid userId, CancellationToken cancellationToken)
    {
        var emailAccount = await dbContext.EmailAccounts.SingleOrDefaultAsync(x => x.Name == hostName && x.EmailAccountUsers.Any(x => x.UserId == userId), cancellationToken);

        if (emailAccount != null)
        {
            var decryptedPassword = await encryptionService.DecryptAsync(Encoding.Unicode.GetBytes(emailAccount.UserPassword), appConfigService.UserEncryptionKey);
            emailAccount.UserPassword = decryptedPassword;
        }

        return emailAccount;
    }

    public async Task<Guid> Create(EmailAccountAddDto emailAccountDto, Guid userId, CancellationToken cancellationToken)
    {
        var encryptedPassword = await encryptionService.EncryptAsync(emailAccountDto.UserPassword, appConfigService.UserEncryptionKey);

        var emailAccount = new EmailAccount
        {
            Id = Guid.NewGuid(),
            Name = emailAccountDto.Name,
            Host = emailAccountDto.Host,
            Port = emailAccountDto.Port,
            UseSsl = emailAccountDto.UseSsl,
            UserName = emailAccountDto.UserName,
            UserPassword = Encoding.Unicode.GetString(encryptedPassword),
            Created = DateTime.UtcNow
        };

        dbContext.EmailAccounts.Add(emailAccount);

        var emailAccountUser = new EmailAccountUser
        {
            EmailAccountId = emailAccount.Id,
            UserId = userId
        };

        dbContext.EmailAccountUsers.Add(emailAccountUser);

        await dbContext.SaveChangesAsync(cancellationToken);

        return emailAccount.Id;
    }

    public async Task<bool> Exists(string name, Guid userId, CancellationToken cancellationToken)
    {
        var emailAccountExists = await dbContext.EmailAccounts.AnyAsync(x => x.Name == name && x.EmailAccountUsers.Any(x => x.UserId == userId), cancellationToken);

        return emailAccountExists;
    }
}
