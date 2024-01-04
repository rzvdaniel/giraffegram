using GG.Auth.Entities;
using GG.Auth.Models;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OpenIddict.EntityFrameworkCore.Models;
using GG.Auth.Dtos;

namespace GG.Auth.Services;

public class ApplicationService(AuthDbContext dbContext, OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager)
{
    public IAsyncEnumerable<OpenIddictEntityFrameworkCoreApplication> List(Guid userId, CancellationToken cancellationToken)
    {
        var applications = applicationManager.ListAsync(apps => 
            apps.Where(app => dbContext.ApplicationUsers
                .Where(x => x.UserId == userId)
                .Select(x => x.ClientId)
                .Contains(app.ClientId)),
                cancellationToken);

        return applications;
    }

    public async ValueTask<ApplicationResultDto?> Get(string clientId, Guid userId, CancellationToken cancellationToken)
    {
        var application = await applicationManager.GetAsync(apps =>
            apps.Where(app => app.ClientId == clientId && 
                dbContext.ApplicationUsers
                .Where(x => x.UserId == userId)
                .Select(x => x.ClientId)
                .Contains(app.ClientId)),
                cancellationToken);

        if (application == null)
            return null;

        var applicationResult = new ApplicationResultDto
        {
            ClientId = application.ClientId,
            Id = application.Id,
            DisplayName = application.DisplayName
        };

        return applicationResult;
    }

    public async Task<object?> Create(ApplicationRegistration applicationRegistration, Guid userId, CancellationToken cancellationToken)
    {
        var existingApplication = await applicationManager.FindByClientIdAsync(applicationRegistration.ClientId, cancellationToken);

        if (existingApplication != null)
        {
            throw new Exception($"Client with id {applicationRegistration.ClientId} already registered");
        }

        var application = await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = applicationRegistration.ClientId,
            ClientSecret = applicationRegistration.ClientPassword,
            DisplayName = applicationRegistration.DisplayName,
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.ClientCredentials
            }
        }, cancellationToken);

        if (application != null)
        {
            var emailHostUser = new ApplicationUser
            {
                ClientId = applicationRegistration.ClientId,
                UserId = userId
            };

            dbContext.ApplicationUsers.Add(emailHostUser);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return application;
    }
}
