using GG.Core.Dto;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace GG.Core.Services;

public class EmailService(EmailAccountService emailHostService, ApplicationDbContext dbContext)
{
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

    public async Task Send(EmailSendDto email, Guid userId, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(email.FromName, email.FromAddress));
        message.To.Add(new MailboxAddress(email.ToName, email.ToAddress));
        message.Subject = email.Subject;

        message.Body = new TextPart("html")
        {
            Text = email.Message
        };

        var emailServer = await emailHostService.Get(email.Server, userId, cancellationToken) ??
            throw new Exception("Email server not found");

        using var client = new SmtpClient();

        client.Connect(emailServer.Host, emailServer.Port, emailServer.UseSsl ?? true, cancellationToken);

        // Note: only needed if the SMTP server requires authentication
        client.Authenticate(emailServer.UserName, emailServer.UserPassword, cancellationToken);

        client.Send(message);
        client.Disconnect(true, cancellationToken);
    }
}
