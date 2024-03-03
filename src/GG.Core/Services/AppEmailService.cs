using GG.Core.Dto;
using System.Web;

namespace GG.Core.Services;

public class AppEmailService(AppConfigService configService, EmailService emailService)
{
    public async Task SendRegistrationEmail(UserRegisterDto userRegistration, Guid userId, CancellationToken cancellationToken)
    {
        var email = new EmailSendDto
        {
            Template = AppEmailTemplates.RegisterUser,
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

        await emailService.Send(email, userId, cancellationToken);
    }

    public async Task SendResetPasswordEmail(UserForgotPasswordDto userForgotPassword, Guid userId, CancellationToken cancellationToken)
    {
        var resetPasswordUrl = $"{configService.AppConfig.WebsiteUrl}/reset-password?email={userForgotPassword.Email}&token={HttpUtility.UrlEncode(userForgotPassword.Token)}";

        var email = new EmailSendDto
        {
            Template = AppEmailTemplates.ResetPassword,
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

        await emailService.Send(email, userId, cancellationToken);
    }
}
