﻿using GG.Core.Dto;
using MailKit.Net.Smtp;
using MimeKit;

namespace GG.Core.Services;

public class EmailService(EmailHostService emailHostService)
{
    public async Task Send(SendEmailDto email, Guid userId, CancellationToken cancellationToken)
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