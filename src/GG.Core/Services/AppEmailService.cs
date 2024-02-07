using GG.Core.Dto;

namespace GG.Core.Services;

public class AppEmailService(ApiKeyService apiKeyService, AppConfigService configService, EmailService emailService)
{
    public async Task SendRegistrationEmail(NewUserDto user, CancellationToken cancellationToken)
    {
        var email = new EmailSendDto
        {
            From = new EmailFrom { Email = configService.EmailConfig.EmailNoReply, Name = "Giraffegram" },
            To = new EmailTo { Email = user.Email, Name = user.Name },
            Template = "Welcome",
            Variables = new Dictionary<string, string>() { { "FullName", user.Name } },
            Account = new EmailAccount
            {
                UserName = configService.EmailConfig.UserName,
                UserPassword = configService.EmailConfig.UserPassword
            },
            Server = new EmailServer
            {
                Host = configService.EmailConfig.ServerHost,
                Port = configService.EmailConfig.ServerPort,
                UseSsl = configService.EmailConfig.ServerUseSsl
            }
        };

        await emailService.Send(email, configService.AppConfig.ApiKey, cancellationToken);
    }
}
