﻿using GG.Portal.Data;
using Microsoft.EntityFrameworkCore;

namespace GG.Portal.Services.EmailTemplate;

public class AppEmailTemplateService(ApplicationDbContext dbContext)
{
    public async Task<IEnumerable<EmailTemplateGetCommand>> List(CancellationToken cancellationToken)
    {
        var emailTemplates = await dbContext.AppEmailTemplates
            .Select(x => new EmailTemplateGetCommand
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

    public async Task<EmailTemplateGetCommand?> Get(Guid id, CancellationToken cancellationToken)
    {
        var emailTemplate = await dbContext.AppEmailTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return emailTemplate is not null ?
            new EmailTemplateGetCommand
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

    public async Task<EmailTemplateGetCommand?> Get(string name, CancellationToken cancellationToken)
    {
        var emailTemplate = await dbContext.AppEmailTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        return emailTemplate is not null ?
            new EmailTemplateGetCommand
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

    public async Task<Guid> Create(EmailTemplateCreateCommand emailAccountDto, CancellationToken cancellationToken)
    {
        var emailTemplate = new AppEmailTemplate
        {
            Id = Guid.NewGuid(),
            Name = emailAccountDto.Name,
            Subject = emailAccountDto.Subject,
            Html = emailAccountDto.Html,
            Created = DateTime.UtcNow
        };

        dbContext.AppEmailTemplates.Add(emailTemplate);

        await dbContext.SaveChangesAsync(cancellationToken);

        return emailTemplate.Id;
    }

    public async Task<bool> Update(Guid id, EmailTemplateUpdateCommand emailTemplateDto, CancellationToken cancellationToken)
    {
        var affected = await dbContext.AppEmailTemplates
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(m => m.Name, emailTemplateDto.Name)
                .SetProperty(m => m.Html, emailTemplateDto.Html)
                .SetProperty(m => m.Subject, emailTemplateDto.Subject)
                .SetProperty(m => m.Updated, DateTime.UtcNow),
                cancellationToken: cancellationToken);

        return affected == 1;
    }

    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        var affected = await dbContext.AppEmailTemplates
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        return affected == 1;
    }

    public async Task<bool> Exists(string name, CancellationToken cancellationToken)
    {
        var emailTemplateExists = await dbContext.AppEmailTemplates.AnyAsync(x => x.Name == name, cancellationToken);

        return emailTemplateExists;
    }
}
