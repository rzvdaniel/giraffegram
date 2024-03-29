﻿using Fluid;
using GG.Portal.Services.Account;
using GG.Portal.Services.AppConfig;
using GG.Portal.Services.Email;
using GG.Portal.Services.EmailTemplate;
using System.Web;

namespace GG.Portal.Services.AppEmail;

public class AppEmailService(AppConfigService configService, EmailService emailService, AppEmailTemplateService emailAppTemplateService)
{
    public async Task SendRegistrationEmail(UserRegistrationEmailCommand userRegistration, CancellationToken cancellationToken)
    {
        var email = new SendEmailCommand
        {
            Template = "RegisterUser",
            From = new EmailAccount
            {
                Email = configService.EmailConfig.Email!
            },
            To = new EmailAccount
            {
                Email = userRegistration.Email,
                Name = userRegistration.Name
            },
            Variables = new Dictionary<string, string>()
            {
                { "FullName", userRegistration.Name }
            },
            Configuration = new EmailConfiguration
            {
                UserName = configService.EmailConfig.UserName!,
                UserPassword = configService.EmailConfig.UserPassword!,
                Host = configService.EmailConfig.ServerHost!,
                Port = configService.EmailConfig.ServerPort,
                UseSsl = configService.EmailConfig.ServerUseSsl
            }
        };

        await SendEmail(email, cancellationToken);
    }

    public async Task SendResetPasswordEmail(UserForgotPasswordCommand userForgotPassword, CancellationToken cancellationToken)
    {
        var resetPasswordUrl = $"{configService.AppConfig.WebsiteUrl}/reset-password?email={userForgotPassword.Email}&token={HttpUtility.UrlEncode(userForgotPassword.Token)}";

        var email = new SendEmailCommand
        {
            Template = "ResetPassword",
            From = new EmailAccount
            {
                Email = configService.EmailConfig.Email!
            },
            To = new EmailAccount
            {
                Email = userForgotPassword.Email,
            },
            Variables = new Dictionary<string, string>()
            {
                { "Name", userForgotPassword.Name??string.Empty },
                { "ResetPasswordUrl", resetPasswordUrl }
            },
            Configuration = new EmailConfiguration
            {
                UserName = configService.EmailConfig.UserName!,
                UserPassword = configService.EmailConfig.UserPassword!,
                Host = configService.EmailConfig.ServerHost!,
                Port = configService.EmailConfig.ServerPort,
                UseSsl = configService.EmailConfig.ServerUseSsl
            }
        };

        await SendEmail(email, cancellationToken);
    }

    private async Task SendEmail(SendEmailCommand emailDto, CancellationToken cancellationToken)
    {
        var renderedEmail = await GetAppEmail(emailDto.Template, emailDto.Variables, cancellationToken);

        if (renderedEmail == null)
            return;

        emailService.SendRenderedEmail(emailDto, renderedEmail, cancellationToken);
    }

    private async Task<EmailResult?> GetAppEmail(string template, Dictionary<string, string> variables, CancellationToken cancellationToken)
    {
        var emailTemplate = await emailAppTemplateService.Get(template, cancellationToken);

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

        foreach (var contextEntry in variables)
        {
            context.SetValue(contextEntry.Key, contextEntry.Value);
        }

        var renderedHtml = htmlTemplate.Render(context);
        var renderedSubject = subjectTemplate.Render(context);

        var renderedEmail = new EmailResult
        {
            Html = renderedHtml,
            Subject = renderedSubject
        };

        return renderedEmail;
    }
}
