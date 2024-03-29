﻿using GG.Auth;
using GG.Auth.Entities;
using GG.Auth.Enums;
using GG.Auth.Models;
using GG.Auth.Resources;
using GG.Auth.Services;
using GG.Core.Models;
using GG.Core.Services;

namespace GG.Api;

public class SetupService(AccountService accountService, 
    AppEmailService appEmailService, 
    AppEmailTemplateService appEmailTemplateService,
    AuthDbContext dbContext)
{
    public async Task Setup(UserRegistration userRegistration, CancellationToken cancellationToken)
    {
       await AddRoles(cancellationToken);

        await AddAdministrator(userRegistration, cancellationToken);

        await AddAppEmailTemplates(cancellationToken);

        var userDetails = new UserDetails { Email = userRegistration.Email, Name = userRegistration.Name };
        await appEmailService.SendRegistrationEmail(userDetails, cancellationToken);
    }

    private async Task AddAdministrator(UserRegistration adminDetails, CancellationToken cancellationToken)
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
                dbContext.Roles.Add(new UserRole { Name = role, NormalizedName = role.ToUpper() });
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
        var registerUserEmail = new EmailTemplateAdd
        {
            Name = RegisterUserEmail.Name,
            Subject = RegisterUserEmail.Subject,
            Html = RegisterUserEmail.Body
        };

        await appEmailTemplateService.Create(registerUserEmail, cancellationToken);
    }

    private async Task AddResetPasswordEmailTemplate(CancellationToken cancellationToken)
    {
        var resetPasswordEmail = new EmailTemplateAdd
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