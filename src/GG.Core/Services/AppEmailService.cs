using GG.Core.Dto;
using Org.BouncyCastle.Security;

namespace GG.Core.Services;

public class AppEmailService(AppConfigService configService, EmailService emailService)
{
    public async Task SendRegistrationEmail(UserRegisterDto user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(configService.AppConfig.ApiKey))
        {
            throw new InvalidParameterException("Api key is not defined in configuration");
        }

        var email = new EmailSendDto
        {
            Template = "Welcome",
            From = new EmailAddress 
            { 
                Email = configService.EmailConfig.Email, 
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

    public async Task SendResetPasswordEmail(UserForgotPasswordDto userForgotPasswordDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(configService.AppConfig.ApiKey))
        {
            throw new InvalidParameterException("Api key is not defined in configuration");
        }

        var resetPasswordUrl = $"{configService.AppConfig.WebsiteUrl}/api/user/reset-password?email={userForgotPasswordDto.Email}&token={userForgotPasswordDto.Token}";

        var email = new EmailSendDto
        {
            Template = "ResetPassword",
            From = new EmailAddress
            {
                Email = configService.EmailConfig.Email,
                Name = "Giraffegram"
            },
            To = new EmailAddress
            {
                Email = userForgotPasswordDto.Email,
            },
            Variables = new Dictionary<string, string>()
            {
                { "Name", userForgotPasswordDto.Name??string.Empty },
                { "ResetPasswordUrl", resetPasswordUrl }
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
