using Fluid;
using GG.Core.Dto;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace GG.Core.Services;

public class EmailService(ApiKeyService apiKeyService, ApplicationDbContext dbContext)
{
    private const string EmailType = "html";

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
        var renderedEmail = await GetEmail(emailDto, apiKey, cancellationToken);

        if (renderedEmail == null)
            return;

        Send(emailDto, renderedEmail, cancellationToken);
    }

    public async Task Send(EmailSendDto emailDto, Guid userId, CancellationToken cancellationToken)
    {
        var renderedEmail = await GetEmail(emailDto, userId, cancellationToken);

        if (renderedEmail == null)
            return;

        Send(emailDto, renderedEmail, cancellationToken);
    }

    private void Send(EmailSendDto emailDto, EmailRenderedDto renderedEmail, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailDto.From.Name, emailDto.From.Email));
        message.To.Add(new MailboxAddress(emailDto.To.Name, emailDto.To.Email));
        message.Subject = renderedEmail.Subject;
        message.Body = new TextPart(EmailType)
        {
            Text = renderedEmail.Html
        };

        using var client = new SmtpClient();
        client.Connect(emailDto.Configuration.Host, emailDto.Configuration.Port, emailDto.Configuration.UseSsl, cancellationToken);

        // Note: only needed if the SMTP server requires authentication
        client.Authenticate(emailDto.Configuration.UserName, emailDto.Configuration.UserPassword, cancellationToken);

        client.Send(message);

        client.Disconnect(true, cancellationToken);
    }

    private async Task<EmailRenderedDto?> GetEmail(EmailSendDto emailDto, string apiKey, CancellationToken cancellationToken)
    {
        var userId = await apiKeyService.GetUserId(apiKey, cancellationToken);

        var renderedEmail = await GetEmail(emailDto, userId, cancellationToken);

        return renderedEmail;
    }

    private async Task<EmailRenderedDto?> GetEmail(EmailSendDto emailDto, Guid userId, CancellationToken cancellationToken)
    {
        var emailTemplate = await dbContext.EmailTemplates.SingleOrDefaultAsync(x => x.Name == emailDto.Template && x.EmailTemplateUsers.Any(x => x.UserId == userId), cancellationToken);

        if (emailTemplate == null)
            return null;

        var parser = new FluidParser();

        if (!parser.TryParse(emailTemplate.Html, out var htmlTemplate, out var error))
        {
            throw new Exception("Could not parse email template body");
        }

        if (!parser.TryParse(emailTemplate.Subject, out var subjectTemplate, out var subjectError))
        {
            throw new Exception("Could not parse email template subject");
        }

        var context = new TemplateContext();

        foreach (var contextEntry in emailDto.Variables)
        {
            context.SetValue(contextEntry.Key, contextEntry.Value);
        }

        var renderedHtml = htmlTemplate.Render(context);
        var renderedSubject = subjectTemplate.Render(context);

        var renderedEmail = new EmailRenderedDto
        {
            Html = renderedHtml,
            Subject = renderedSubject
        };

        return renderedEmail;
    }
}
