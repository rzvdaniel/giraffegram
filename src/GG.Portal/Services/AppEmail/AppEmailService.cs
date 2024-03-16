using Fluid;
using GG.Portal.Services.AppConfig;
using GG.Portal.Services.Email;
using GG.Portal.Services.EmailTemplate;
using System.Web;

namespace GG.Portal.Services.AppEmail;

public class AppEmailService(AppConfigService configService, EmailService emailService, AppEmailTemplateService emailAppTemplateService)
{
    public async Task SendRegistrationEmail(AppUserDetails userRegistration, CancellationToken cancellationToken)
    {
        var email = new EmailSend
        {
            Template = "RegisterUser",
            From = new EmailAddress
            {
                Email = configService.EmailConfig.Email!
            },
            To = new EmailAddress
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

    public async Task SendResetPasswordEmail(AppUserForgotPassword userForgotPassword, CancellationToken cancellationToken)
    {
        var resetPasswordUrl = $"{configService.AppConfig.WebsiteUrl}/reset-password?email={userForgotPassword.Email}&token={HttpUtility.UrlEncode(userForgotPassword.Token)}";

        var email = new EmailSend
        {
            Template = "ResetPassword",
            From = new EmailAddress
            {
                Email = configService.EmailConfig.Email!
            },
            To = new EmailAddress
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

    private async Task SendEmail(EmailSend emailDto, CancellationToken cancellationToken)
    {
        var renderedEmail = await GetAppEmail(emailDto, cancellationToken);

        if (renderedEmail == null)
            return;

        emailService.SendRenderedEmail(emailDto, renderedEmail, cancellationToken);
    }

    private async Task<EmailRendered?> GetAppEmail(EmailSend emailDto, CancellationToken cancellationToken)
    {
        var emailTemplate = await emailAppTemplateService.Get(emailDto.Template, cancellationToken);

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

        var renderedEmail = new EmailRendered
        {
            Html = renderedHtml,
            Subject = renderedSubject
        };

        return renderedEmail;
    }
}
