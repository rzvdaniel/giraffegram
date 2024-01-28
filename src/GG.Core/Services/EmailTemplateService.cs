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
                Text = x.Text,
                Html = x.Html,
                Created = x.Created,
                Updated = x.Updated
            })
            .ToListAsync(cancellationToken);

        return emailTemplates;
    }

    public async Task<EmailTemplateGetDto?> Get(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplate = await dbContext.EmailTemplates.SingleOrDefaultAsync(x => x.Id == id && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        if (emailTemplate == null) 
            return null; 

        var emailTemplateGetDto = new EmailTemplateGetDto
        {
            Id = emailTemplate.Id,
            Name = emailTemplate.Name,
            Text = emailTemplate.Text,
            Html = emailTemplate.Html,
            Created = emailTemplate.Created,
            Updated = emailTemplate.Updated
        };

        return emailTemplateGetDto;
    }

    public async Task<EmailTemplateGetDto?> Get(string emailTemplateName, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplate = await dbContext.EmailTemplates.SingleOrDefaultAsync(x => x.Name == emailTemplateName && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        if (emailTemplate == null)
            return null;

        var emailTemplateGetDto = new EmailTemplateGetDto
        {
            Id = emailTemplate.Id,
            Name = emailTemplate.Name,
            Text = emailTemplate.Text,
            Html = emailTemplate.Html,
            Created = emailTemplate.Created,
            Updated = emailTemplate.Updated
        };

        return emailTemplateGetDto;
    }

    public async Task<Guid> Create(EmailTemplateAddDto emailAccountDto, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplate = new EmailTemplate
        {
            Id = Guid.NewGuid(),
            Name = emailAccountDto.Name,
            Text = emailAccountDto.Text,
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

    public void Update(Guid id, EmailTemplateUpdateDto emailTemplateDto, Guid userId)
    {
        var emailTemplate = dbContext.EmailTemplates
            .Include(x => x.EmailTemplateUsers)
            .SingleOrDefault(x => x.Id == id && x.EmailTemplateUsers.Any(x => x.UserId == userId));

        if (emailTemplate == null)
            return;

        emailTemplate.Name = emailTemplateDto.Name;
        emailTemplate.Text = emailTemplateDto.Text;
        emailTemplate.Html = emailTemplateDto.Html;
        emailTemplate.Updated = DateTime.UtcNow;

        dbContext.SaveChanges();
    }

    public void Delete(Guid id, Guid userId)
    {
        var emailTemplate = dbContext.EmailTemplates.SingleOrDefault(x => x.Id == id && x.EmailTemplateUsers.Any(x => x.UserId == userId));

        if (emailTemplate == null)
            return;

        dbContext.EmailTemplates.Remove(emailTemplate);

        dbContext.SaveChanges();
    }

    public async Task<bool> Exists(string name, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplateExists = await dbContext.EmailTemplates.AnyAsync(x => x.Name == name && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        return emailTemplateExists;
    }
}
