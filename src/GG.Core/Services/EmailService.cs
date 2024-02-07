using Fluid;
using GG.Core.Dto;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace GG.Core.Services;

public class EmailService(ApiKeyService apiKeyService, ApplicationDbContext dbContext)
{
    public async Task<string?> Get(string emailTemplateName, string apiKey, Dictionary<string, string> contextData, CancellationToken cancellationToken)
    {
        var userId = await apiKeyService.GetUserId(apiKey, cancellationToken);

        var emailTemplate = await dbContext.EmailTemplates.SingleOrDefaultAsync(x => x.Name == emailTemplateName && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        if (emailTemplate == null)
            return null;

        var emailTemplateGetDto = new EmailTemplateGetDto
        {
            Id = emailTemplate.Id,
            Name = emailTemplate.Name,
            Html = emailTemplate.Html,
            Subject = emailTemplate.Subject,
            Created = emailTemplate.Created,
            Updated = emailTemplate.Updated
        };

        var parser = new FluidParser();

        if (!parser.TryParse(emailTemplateGetDto.Html, out var template, out var error))
        {
            throw new Exception("Could not parse email template");
        }

        var context = new TemplateContext();

        foreach (var contextEntry in contextData)
        {
            context.SetValue(contextEntry.Key, contextEntry.Value);
        }

        var emailHtml = template.Render(context);

        return emailHtml;
    }

    public async Task Send(EmailSendDto emailDto, string apiKey, CancellationToken cancellationToken)
    {
        var body = await GetEmail(emailDto, apiKey, cancellationToken);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailDto.From.Name, emailDto.From.Email));
        message.To.Add(new MailboxAddress(emailDto.To.Name, emailDto.To.Email));
        message.Subject = "Welcome!!!";
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

        client.Connect(emailDto.Server.Host, emailDto.Server.Port, emailDto.Server.UseSsl, cancellationToken);

        // Note: only needed if the SMTP server requires authentication
        client.Authenticate(emailDto.Account.UserName, emailDto.Account.UserPassword, cancellationToken);

        client.Send(message);

        client.Disconnect(true, cancellationToken);
    }

    private async Task<string?> GetEmail(EmailSendDto emailDto, string apiKey, CancellationToken cancellationToken)
    {
        var userId = await apiKeyService.GetUserId(apiKey, cancellationToken);

        var emailTemplate = await dbContext.EmailTemplates.SingleOrDefaultAsync(x => x.Name == emailDto.Template && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        if (emailTemplate == null)
            return null;

        var emailBody = emailTemplate.Html;

        var parser = new FluidParser();

        if (!parser.TryParse(emailBody, out var template, out var error))
        {
            throw new Exception("Could not parse email template");
        }

        var context = new TemplateContext();

        foreach (var contextEntry in emailDto.Variables)
        {
            context.SetValue(contextEntry.Key, contextEntry.Value);
        }

        var email = template.Render(context);

        return email;
    }
}
