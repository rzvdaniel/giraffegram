using GG.Core.Dto;

namespace GG.Core.Services;

public class AppEmailService(ApiKeyService apiKeyService, AppConfigService configService, EmailService emailService)
{
    public async Task SendRegistrationEmail(UserRegisterDto user, CancellationToken cancellationToken)
    {
        var email = new EmailSendDto
        {
            Template = "Welcome",
            From = new EmailAddress 
            { 
                Email = configService.EmailConfig.EmailNoReply, 
                Name = "Giraffegram" 
            },
            To = new EmailAddress
            { 
                Email = user.Email, 
                Name = user.Name 
            },
            Variables = new Dictionary<string, string>() 
            { 
                { "FullName", user.Name } 
            },
            Configuration = new EmailConfiguration
            {
                UserName = configService.EmailConfig.UserName,
                UserPassword = configService.EmailConfig.UserPassword,
                Host = configService.EmailConfig.ServerHost,
                Port = configService.EmailConfig.ServerPort,
                UseSsl = configService.EmailConfig.ServerUseSsl
            }
        };

        await emailService.Send(email, configService.AppConfig.ApiKey, cancellationToken);
    }

    public async Task SendResetPasswordEmail(string emailAddress, string resetPasswordToken, CancellationToken cancellationToken)
    {
        var email = new EmailSendDto
        {
            Template = "ResetPassword",
            From = new EmailAddress
            {
                Email = configService.EmailConfig.EmailNoReply,
                Name = "Giraffegram"
            },
            To = new EmailAddress
            {
                Email = emailAddress,
            },
            Variables = new Dictionary<string, string>()
            {
                { "Token", resetPasswordToken }
            },
            Configuration = new EmailConfiguration
            {
                UserName = configService.EmailConfig.UserName,
                UserPassword = configService.EmailConfig.UserPassword,
                Host = configService.EmailConfig.ServerHost,
                Port = configService.EmailConfig.ServerPort,
                UseSsl = configService.EmailConfig.ServerUseSsl
            }
        };

        await emailService.Send(email, configService.AppConfig.ApiKey, cancellationToken);
    }
}
