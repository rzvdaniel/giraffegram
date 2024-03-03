using GG.Auth;
using GG.Auth.Entities;
using GG.Auth.Enums;
using GG.Auth.Resources;
using GG.Auth.Services;
using GG.Core.Dto;

namespace GG.Core.Services;

public class SetupService(AccountService accountService, 
    AppEmailService appEmailService, 
    EmailTemplateService emailTemplateService,
    AuthDbContext dbContext)
{
    public async Task Setup(UserRegisterDto userRegistration, CancellationToken cancellationToken)
    {
       await AddRoles(cancellationToken);

        var admin = await AddAdministrator(userRegistration, cancellationToken);

        await AddAdminEmails(admin.Id, cancellationToken);

        await appEmailService.SendRegistrationEmail(userRegistration, admin.Id, cancellationToken);
    }

    private async Task<User> AddAdministrator(UserRegisterDto adminDetails, CancellationToken cancellationToken)
    {
        var result = await accountService.CreateUser(adminDetails, cancellationToken);

        if (!result.Succeeded)
            throw new Exception("Could not create administrator");

        var user = await accountService.GetUserByEmailOrUserName(adminDetails.Email) ?? 
            throw new Exception("Could not find administrator");

        await accountService.AddUserToRole(user.Id, UserRoles.Administrator);

        return user;
    }

    public async Task AddRoles(CancellationToken cancellationToken)
    {
        var roles = new string[] { UserRoles.Administrator };

        foreach (string role in roles)
        {
            if (!dbContext.Roles.Any(r => r.Name == role))
            {
                dbContext.Roles.Add(new UserRole { Name = role, NormalizedName = role.ToUpper() });
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsSetupComplete()
    {
        return await accountService.AdminExists();
    }

    private async Task AddRegisterUserEmail(Guid adminId, CancellationToken cancellationToken)
    {
        var registerUserEmail = new EmailTemplateAddDto
        {
            Name = RegisterUserEmail.Name,
            Subject = RegisterUserEmail.Subject,
            Html = RegisterUserEmail.Body
        };

        await emailTemplateService.Create(registerUserEmail, adminId, cancellationToken);
    }

    private async Task AddResetPasswordEmail(Guid adminId, CancellationToken cancellationToken)
    {
        var resetPasswordEmail = new EmailTemplateAddDto
        {
            Name = ResetPasswordEmail.Name,
            Subject = ResetPasswordEmail.Subject,
            Html = ResetPasswordEmail.Body
        };

        await emailTemplateService.Create(resetPasswordEmail, adminId, cancellationToken);
    }

    private async Task AddAdminEmails(Guid adminId, CancellationToken cancellationToken)
    {
        await AddRegisterUserEmail(adminId, cancellationToken);
        await AddResetPasswordEmail(adminId, cancellationToken);
    }
}