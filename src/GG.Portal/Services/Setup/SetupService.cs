using GG.Auth.Entities;
using GG.Portal.Data;
using GG.Portal.Enums;
using GG.Portal.Resources;
using GG.Portal.Services.Account;
using GG.Portal.Services.AppEmail;
using GG.Portal.Services.EmailTemplate;

namespace GG.Portal.Services.Setup;

public class SetupService(
    AccountService accountService,
    AppEmailService appEmailService,
    AppEmailTemplateService appEmailTemplateService,
    ApplicationDbContext dbContext)
{
    public async Task Setup(UserRegistrationCommand userRegistration, CancellationToken cancellationToken)
    {
        await AddRoles(cancellationToken);

        await AddAdministrator(userRegistration, cancellationToken);

        await AddAppEmailTemplates(cancellationToken);

        var userDetails = new UserRegistrationEmailCommand { Email = userRegistration.Email, Name = userRegistration.Name };
        await appEmailService.SendRegistrationEmail(userDetails, cancellationToken);
    }

    private async Task AddAdministrator(UserRegistrationCommand adminDetails, CancellationToken cancellationToken)
    {
        var result = await accountService.CreateUser(adminDetails, cancellationToken);

        if (!result.Succeeded)
            throw new Exception("Could not create administrator");

        var admin = await accountService.GetUserByEmailOrUserName(adminDetails.Email) ??
            throw new Exception("Could not find administrator");

        await accountService.AddUserToRole(admin.Id, UserRoles.Administrator);
    }

    public async Task AddRoles(CancellationToken cancellationToken)
    {
        var roles = new string[] { UserRoles.Administrator, UserRoles.User };

        foreach (string role in roles)
        {
            if (!dbContext.Roles.Any(r => r.Name == role))
            {
                dbContext.Roles.Add(new UserRoleEntity { Name = role, NormalizedName = role.ToUpper() });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsSetupComplete()
    {
        return await accountService.AdminExists();
    }

    private async Task AddRegisterUserEmailTemplate(CancellationToken cancellationToken)
    {
        var registerUserEmail = new EmailTemplateCreateCommand
        {
            Name = RegisterUserEmail.Name,
            Subject = RegisterUserEmail.Subject,
            Html = RegisterUserEmail.Body
        };

        await appEmailTemplateService.Create(registerUserEmail, cancellationToken);
    }

    private async Task AddResetPasswordEmailTemplate(CancellationToken cancellationToken)
    {
        var resetPasswordEmail = new EmailTemplateCreateCommand
        {
            Name = ResetPasswordEmail.Name,
            Subject = ResetPasswordEmail.Subject,
            Html = ResetPasswordEmail.Body
        };

        await appEmailTemplateService.Create(resetPasswordEmail, cancellationToken);
    }

    private async Task AddAppEmailTemplates(CancellationToken cancellationToken)
    {
        await AddRegisterUserEmailTemplate(cancellationToken);
        await AddResetPasswordEmailTemplate(cancellationToken);
    }
}