using Fluid;
using GG.Core.Dto;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            Text = emailTemplate.Text,
            Html = emailTemplate.Html,
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

    public async Task Send(EmailSendDto email, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(email.FromName, email.FromAddress));
        message.To.Add(new MailboxAddress(email.ToName, email.ToAddress));
        message.Subject = email.Subject;
        message.Body = !string.IsNullOrEmpty(email.Html) ?
            new TextPart("html") { Text = email.Html } :
            new TextPart("text") { Text = email.Text };

        using var client = new SmtpClient();

        client.Connect(email.Host, email.Port, email.UseSsl, cancellationToken);

        // Note: only needed if the SMTP server requires authentication
        client.Authenticate(email.UserName, email.UserPassword, cancellationToken);

        client.Send(message);

        client.Disconnect(true, cancellationToken);
    }
}
