using GG.Core.Dto;
using GG.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GG.Core.Services;

public class EmailTemplateService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<EmailTemplateGetDto>> List(Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplates = await dbContext.EmailTemplates
            .Where(x => x.EmailTemplateUsers.Any(x => x.UserId == userId))
            .Select(x => new EmailTemplateGetDto
            {
                Id = x.Id,
                Name = x.Name,
                Html = x.Html,
                Subject = x.Subject,
                Created = x.Created,
                Updated = x.Updated
            })
            .ToListAsync(cancellationToken);

        return emailTemplates;
    }

    public async Task<EmailTemplateGetDto?> Get(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplate = await dbContext.EmailTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        return emailTemplate is not null ? 
            new EmailTemplateGetDto
            {
                Id = emailTemplate.Id,
                Name = emailTemplate.Name,
                Subject = emailTemplate.Subject,
                Html = emailTemplate.Html,
                Created = emailTemplate.Created,
                Updated = emailTemplate.Updated
            } : 
            null;
    }

    public async Task<Guid> Create(EmailTemplateAddDto emailAccountDto, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplate = new EmailTemplate
        {
            Id = Guid.NewGuid(),
            Name = emailAccountDto.Name,
            Subject = emailAccountDto.Subject,
            Html = emailAccountDto.Html,
            Created = DateTime.UtcNow
        };

        dbContext.EmailTemplates.Add(emailTemplate);

        var emailTemplateUser = new EmailTemplateUser
        {
            EmailTemplateId = emailTemplate.Id,
            UserId = userId
        };

        dbContext.EmailTemplateUsers.Add(emailTemplateUser);

        await dbContext.SaveChangesAsync(cancellationToken);

        return emailTemplate.Id;
    }

    public async Task<bool> Update(Guid id, EmailTemplateUpdateDto emailTemplateDto, Guid userId, CancellationToken cancellationToken)
    {
        var affected = await dbContext.EmailTemplates
            .Include(x => x.EmailTemplateUsers)
            .Where(x => x.Id == id && x.EmailTemplateUsers.Any(x => x.UserId == userId))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Name, emailTemplateDto.Name)
                .SetProperty(m => m.Html, emailTemplateDto.Html)
                .SetProperty(m => m.Subject, emailTemplateDto.Subject)
                .SetProperty(m => m.Updated, DateTime.UtcNow), 
                cancellationToken: cancellationToken);

        return affected == 1;
    }

    public async Task<bool> Delete(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var affected = await dbContext.EmailTemplates
            .Where(x => x.Id == id && x.EmailTemplateUsers.Any(x => x.UserId == userId))
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return affected == 1;
    }

    public async Task<bool> Exists(string name, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplateExists = await dbContext.EmailTemplates.AnyAsync(x => x.Name == name && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        return emailTemplateExists;
    }
}
